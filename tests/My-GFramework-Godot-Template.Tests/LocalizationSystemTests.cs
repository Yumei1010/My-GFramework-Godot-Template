using System;
using System.Collections.Generic;
using System.IO;
using GFramework.Core.Abstractions.Localization;
using GFramework.Core.Localization;

namespace GFrameworkTemplate.Tests;

/// <summary>
///     本地化系统验证：文档 docs/guides/localization.md 的真实机制（文件驱动）。
///     从临时目录加载语言表 JSON，验证懒加载/回退链/变量插值/订阅。
/// </summary>
public class LocalizationSystemTests : IDisposable
{
    private readonly string _dataPath;

    public LocalizationSystemTests()
    {
        _dataPath = Path.Combine(Path.GetTempPath(), $"gfw_loc_{Guid.NewGuid():N}");
        CreateLang("zh", new Dictionary<string, string>
        {
            ["hp"] = "生命值",
            ["enemy"] = "敌人",
            ["hit"] = "击中 {count} 次",
            ["gold"] = "金币: {gold:compact}"
        });
        CreateLang("en", new Dictionary<string, string>
        {
            ["hp"] = "HP",
            ["enemy"] = "Enemy"
            // 故意缺 hit/gold → 回退 zh
        });
    }

    public void Dispose()
    {
        if (Directory.Exists(_dataPath))
        {
            Directory.Delete(_dataPath, recursive: true);
        }
    }

    private void CreateLang(string lang, Dictionary<string, string> entries)
    {
        var dir = Path.Combine(_dataPath, lang);
        Directory.CreateDirectory(dir);
        var json = System.Text.Json.JsonSerializer.Serialize(entries);
        File.WriteAllText(Path.Combine(dir, "main.json"), json);
    }

    private LocalizationManager BuildManager(string defaultLang = "zh")
    {
        var manager = new LocalizationManager(new LocalizationConfig
        {
            DefaultLanguage = defaultLang,
            FallbackLanguage = "zh",
            LocalizationPath = _dataPath,
            EnableHotReload = false,
            ValidateOnLoad = false
        });
        manager.Initialize();
        return manager;
    }

    [Fact]
    public void 默认语言_取词正确()
    {
        var manager = BuildManager("zh");
        Assert.Equal("zh", manager.CurrentLanguage);
        Assert.Equal("生命值", manager.GetText("main", "hp"));
    }

    [Fact]
    public void 切换语言_懒加载英文表()
    {
        var manager = BuildManager("zh");
        manager.SetLanguage("en");
        Assert.Equal("en", manager.CurrentLanguage);
        Assert.Equal("HP", manager.GetText("main", "hp"));
    }

    [Fact]
    public void 回退链_英文缺词回退中文()
    {
        var manager = BuildManager("en");   // 默认英文
        // en 表缺 "hit" → 回退 zh
        Assert.Equal("击中 {count} 次", manager.GetText("main", "hit"));
    }

    [Fact]
    public void 变量插值_替换占位符()
    {
        var manager = BuildManager("zh");
        var str = manager.GetString("main", "hit").WithVariable("count", 3);
        Assert.Equal("击中 3 次", str.Format());   // 取值用 Format()
    }

    [Fact]
    public void 格式化器_compact在文案中生效()
    {
        var manager = BuildManager("zh");
        var str = manager.GetString("main", "gold").WithVariable("gold", 1234);
        var result = str.Format();
        Assert.Contains("金币", result);
        Assert.DoesNotContain("{gold", result);   // 变量被替换/格式化
    }

    [Fact]
    public void 订阅语言变更_收到通知()
    {
        var manager = BuildManager("zh");
        string? received = null;
        manager.SubscribeToLanguageChange(code => received = code);
        manager.SetLanguage("en");
        Assert.Equal("en", received);
    }

    [Fact]
    public void TryGetText_缺词返回false()
    {
        var manager = BuildManager("en");
        Assert.False(manager.TryGetText("main", "no_such_key", out _));
        Assert.True(manager.TryGetText("main", "hp", out var hp));
        Assert.Equal("HP", hp);
    }

    [Fact]
    public void AvailableLanguages_含已载语言()
    {
        var manager = BuildManager("zh");
        manager.SetLanguage("en");
        Assert.Contains("zh", manager.AvailableLanguages);
        Assert.Contains("en", manager.AvailableLanguages);
    }
}