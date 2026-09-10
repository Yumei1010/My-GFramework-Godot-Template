using System;
using System.IO;
using Godot;
using FileAccess = Godot.FileAccess;

namespace GFrameworkTemplate.scripts.core.localization;

/// <summary>
///     本地化数据路径解析器：为框架 <c>LocalizationManager</c> 准备"真实文件系统路径"。
/// </summary>
/// <remarks>
///     框架 <c>LocalizationManager</c> 使用 <c>System.IO</c> 读取语言表，不识别 Godot 的 <c>res://</c> 虚拟路径：
///     <list type="bullet">
///         <item>编辑器态：<c>res://localization</c> 直接映射到项目目录，可直接读取</item>
///         <item>导出态：<c>res://</c> 位于 pck 内，<c>System.IO</c> 读不到——先同步到 <c>user://localization</c>（真实文件系统）再读取</item>
///     </list>
///     探测方式为"实测真实目录是否存在"，不依赖编辑器特性开关，编辑器/导出行为一致可预期。
/// </remarks>
public static class LocalizationPathResolver
{
    private const string ResRoot = "res://localization";
    private const string UserRoot = "user://localization";

    /// <summary>
    ///     解析本地化数据根目录（真实文件系统路径）。
    /// </summary>
    /// <returns>可直接交给 <c>LocalizationConfig.LocalizationPath</c> 的真实路径。</returns>
    public static string Resolve()
    {
        var resRealPath = ProjectSettings.GlobalizePath(ResRoot);
        if (Directory.Exists(resRealPath))
        {
            return resRealPath;
        }

        SynchronizeToUser();
        return ProjectSettings.GlobalizePath(UserRoot);
    }

    /// <summary>
    ///     把 <c>res://localization</c> 下的 JSON 语言表同步到 <c>user://localization</c>。
    /// </summary>
    private static void SynchronizeToUser()
    {
        if (!DirAccess.DirExistsAbsolute(ResRoot))
        {
            GD.PushWarning($"[Localization] 未找到 {ResRoot}，本地化数据为空。");
            return;
        }

        DirAccess.MakeDirRecursiveAbsolute(UserRoot);
        CopyJsonRecursive(ResRoot, UserRoot);
    }

    private static void CopyJsonRecursive(string sourceDir, string targetDir)
    {
        using var dir = DirAccess.Open(sourceDir);
        if (dir is null)
        {
            return;
        }

        dir.ListDirBegin();
        for (var name = dir.GetNext(); !string.IsNullOrEmpty(name); name = dir.GetNext())
        {
            if (name.StartsWith('.'))
            {
                continue;
            }

            var sourcePath = $"{sourceDir}/{name}";
            var targetPath = $"{targetDir}/{name}";

            if (dir.CurrentIsDir())
            {
                DirAccess.MakeDirRecursiveAbsolute(targetPath);
                CopyJsonRecursive(sourcePath, targetPath);
                continue;
            }

            if (!name.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            // FileAccess 可读 res://（含导出 pck），写入真实 user:// 目录供 System.IO 使用
            using var writer = FileAccess.Open(targetPath, FileAccess.ModeFlags.Write);
            writer?.StoreString(FileAccess.GetFileAsString(sourcePath));
        }

        dir.ListDirEnd();
    }
}