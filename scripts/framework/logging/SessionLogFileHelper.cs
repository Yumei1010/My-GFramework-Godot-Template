using System;
using System.IO;

namespace GFrameworkTemplate.scripts.framework.logging;

/// <summary>
///     会话日志文件创建辅助类，负责生成按会话切分的日志文件路径。
/// </summary>
/// <remarks>
///     文件名格式：<c>yyyyMMdd_HHmmss.log</c>（如 <c>20260907_153000.log</c>）。
///     本类只做路径计算与目录创建，不依赖 Godot API，可独立单测。
/// </remarks>
public static class SessionLogFileHelper
{
    private const string LogFileExtension = ".log";

    /// <summary>
    ///     创建本次运行使用的会话日志文件信息。
    /// </summary>
    /// <param name="logDirectoryPath">日志目录路径（真实文件系统路径，非 <c>user://</c>）。</param>
    /// <param name="timestamp">会话时间戳，用于生成文件名。</param>
    /// <returns>会话日志文件信息。</returns>
    public static SessionLogFileInfo Create(string logDirectoryPath, DateTime timestamp)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(logDirectoryPath);

        Directory.CreateDirectory(logDirectoryPath);

        var fileName = timestamp.ToString("yyyyMMdd_HHmmss") + LogFileExtension;
        var fullPath = Path.Combine(logDirectoryPath, fileName);

        return new SessionLogFileInfo(fullPath, logDirectoryPath, fileName);
    }

    /// <summary>
    ///     清理历史会话日志，只保留最近的若干个文件。
    /// </summary>
    /// <param name="logDirectoryPath">日志目录（真实文件系统路径）。</param>
    /// <param name="keepCount">保留的最新会话文件数量（按文件名时间戳倒序，&lt;= 0 表示全部清理）。</param>
    /// <returns>实际删除的文件数量。</returns>
    /// <remarks>
    ///     文件名格式为 <c>yyyyMMdd_HHmmss.log</c>，因此字典序即时间序，无需读取文件时间。
    ///     被占用而删除失败的文件会被跳过，不影响启动。
    /// </remarks>
    public static int PruneOldSessions(string logDirectoryPath, int keepCount = 20)
    {
        if (string.IsNullOrWhiteSpace(logDirectoryPath) || !Directory.Exists(logDirectoryPath))
        {
            return 0;
        }

        var expiredFiles = Directory
            .GetFiles(logDirectoryPath, "*" + LogFileExtension, SearchOption.TopDirectoryOnly)
            .OrderByDescending(static path => path, StringComparer.Ordinal)
            .Skip(Math.Max(0, keepCount))
            .ToArray();

        var deleted = 0;
        foreach (var file in expiredFiles)
        {
            try
            {
                File.Delete(file);
                deleted++;
            }
            catch (IOException)
            {
                // 文件被其他进程占用（例如正在查看）：跳过，下次启动再清理
            }
            catch (UnauthorizedAccessException)
            {
                // 无权限：跳过
            }
        }

        return deleted;
    }

    /// <summary>
    ///     使用当前时间创建会话日志文件信息。
    /// </summary>
    /// <param name="logDirectoryPath">日志目录路径（真实文件系统路径）。</param>
    /// <returns>会话日志文件信息。</returns>
    public static SessionLogFileInfo CreateNow(string logDirectoryPath)
    {
        return Create(logDirectoryPath, DateTime.Now);
    }
}
