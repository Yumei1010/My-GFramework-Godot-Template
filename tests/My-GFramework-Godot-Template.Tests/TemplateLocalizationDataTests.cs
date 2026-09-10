using System;
using System.IO;
using GFramework.Core.Abstractions.Localization;
using GFramework.Core.Localization;

namespace GFrameworkTemplate.Tests;

/// <summary>
///     模板本地化数据验证：确认模板自带 localization/ 目录的语言表可被加载与切换。
///     使用仓库根真实路径（非 Godot 虚拟路径），不依赖 Godot 运行时。
/// </summary>
public class TemplateLocalizationDataTests
{
    private static string FindRepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is { Parent: not null })
        {
            if (Directory.Exists(Path.Combine(dir.FullName, "localization")))
            {
                return dir.FullName;
            }

            dir = dir.Parent;
        }

        throw new DirectoryNotFoundException("localization 目录未找到（仓库根）");
    }

    private static LocalizationManager CreateManager()
    {
        var manager = new LocalizationManager(new LocalizationConfig
        {
            DefaultLanguage = "zhs",
            FallbackLanguage = "eng",
            LocalizationPath = Path.Combine(FindRepoRoot(), "localization"),
            EnableHotReload = false,
            ValidateOnLoad = false
        });
        manager.Initialize();
        return manager;
    }

    [Fact]
    public void 模板数据_中英文表均可加载()
    {
        var manager = CreateManager();

        Assert.Equal("GFramework 模板", manager.GetText("common", "app.title"));

        manager.SetLanguage("eng");
        Assert.Equal("GFramework Template", manager.GetText("common", "app.title"));
    }

    [Fact]
    public void 模板数据_两语言键一致()
    {
        var manager = CreateManager();

        Assert.True(manager.TryGetText("common", "ui.confirm", out var zh));
        manager.SetLanguage("eng");
        Assert.True(manager.TryGetText("common", "ui.confirm", out var en));
        Assert.NotEqual(zh, en);
    }

    [Fact]
    public void 模板数据_语言目录按框架语言码组织()
    {
        var root = Path.Combine(FindRepoRoot(), "localization");

        Assert.True(File.Exists(Path.Combine(root, "zhs", "common.json")));
        Assert.True(File.Exists(Path.Combine(root, "eng", "common.json")));
    }
}