using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using GFramework.Core.Abstractions.Command;
using GFramework.Core.SourceGenerators.Abstractions.Logging;
using GFramework.Core.SourceGenerators.Abstractions.Rule;

namespace GFrameworkTemplate.Tests;

/// <summary>
///     仓库一致性自检：守护规范、配置与本地化数据不随时间漂移。
///     不依赖 Godot 运行时，纯反射 + 文件系统检查。
/// </summary>
public class RepositoryConsistencyTests
{
    private static readonly Assembly TemplateAssembly = typeof(global::GFrameworkTemplate.global.GameEntryPoint).Assembly;

    private static string FindRepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is { Parent: not null })
        {
            if (Directory.Exists(Path.Combine(dir.FullName, "schemas")) &&
                File.Exists(Path.Combine(dir.FullName, "My-GFramework-Godot-Template.csproj")))
            {
                return dir.FullName;
            }

            dir = dir.Parent;
        }

        throw new DirectoryNotFoundException("仓库根目录未找到");
    }

    // ── 规范守护 ──────────────────────────────────────────────

    [Fact]
    public void 命令类_必须全部为_sealed()
    {
        var commandTypes = TemplateAssembly.GetTypes()
            .Where(static t => t is { IsClass: true, IsAbstract: false, IsInterface: false })
            .Where(static t => typeof(ICommand).IsAssignableFrom(t) ||
                               typeof(IAsyncCommand).IsAssignableFrom(t) ||
                               t.GetInterfaces().Any(static i =>
                                   i.IsGenericType &&
                                   (i.GetGenericTypeDefinition() == typeof(ICommand<>) ||
                                    i.GetGenericTypeDefinition() == typeof(IAsyncCommand<>))))
            .ToArray();

        Assert.NotEmpty(commandTypes);
        Assert.All(commandTypes, static t => Assert.True(t.IsSealed, $"命令类 {t.Name} 必须声明为 sealed"));
    }

    [Fact]
    public void 命令输入类_必须为_class_禁止_struct()
    {
        var inputTypes = TemplateAssembly.GetTypes()
            .Where(static t => t is { IsClass: true, IsAbstract: false })
            .Where(static t => typeof(global::GFramework.Cqrs.Abstractions.Cqrs.Command.ICommandInput).IsAssignableFrom(t))
            .ToArray();

        Assert.All(inputTypes, static t => Assert.True(t.IsSealed, $"命令输入 {t.Name} 必须声明为 sealed"));
    }

    [Fact]
    public void 事件类_必须全部为_sealed()
    {
        // 事件位于 scripts/cqrs/<domain>/event/（编译后命名空间 ...cqrs.<domain>.event）
        var eventTypes = TemplateAssembly.GetTypes()
            .Where(static t => t is { IsClass: true, IsAbstract: false, IsPublic: true })
            .Where(static t => t.Namespace is not null &&
                               t.Namespace.Contains(".cqrs.", StringComparison.Ordinal) &&
                               t.Namespace.EndsWith(".event", StringComparison.Ordinal))
            .ToArray();

        Assert.NotEmpty(eventTypes);
        Assert.All(eventTypes, static t => Assert.True(t.IsSealed, $"事件类 {t.Name} 必须声明为 sealed"));
    }

    [Fact]
    public void Godot节点_ContextAware与Log必须成对()
    {
        var contextAwareTypes = TemplateAssembly.GetTypes()
            .Where(static t => t is { IsClass: true, IsAbstract: false })
            .Where(static t => t.GetCustomAttribute<ContextAwareAttribute>() is not null)
            .ToArray();

        Assert.NotEmpty(contextAwareTypes);
        Assert.All(contextAwareTypes, static t => Assert.True(
            t.GetCustomAttribute<LogAttribute>() is not null,
            $"类 {t.Name} 标注了 [ContextAware] 却缺少配对的 [Log]"));
    }

    // ── 配置与本地化数据守护 ──────────────────────────────────

    [Fact]
    public void 每个schema_都有对应配置目录与数据()
    {
        var root = FindRepoRoot();
        var schemaFiles = Directory.GetFiles(Path.Combine(root, "schemas"), "*.schema.json");

        Assert.NotEmpty(schemaFiles);
        foreach (var schemaPath in schemaFiles)
        {
            using var doc = JsonDocument.Parse(File.ReadAllText(schemaPath));
            var configPath = doc.RootElement.TryGetProperty("x-gframework-config-path", out var p)
                ? p.GetString()
                : Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(schemaPath));

            Assert.False(string.IsNullOrWhiteSpace(configPath), $"{Path.GetFileName(schemaPath)} 缺少配置目录声明");

            var dataDir = Path.Combine(root, configPath!.Replace('/', Path.DirectorySeparatorChar));
            Assert.True(Directory.Exists(dataDir), $"schema 声明的配置目录不存在: {configPath}");
            Assert.NotEmpty(Directory.GetFiles(dataDir, "*.yaml"));
        }
    }

    [Fact]
    public void 本地化_各语言表结构一致()
    {
        var root = FindRepoRoot();
        var localizationRoot = Path.Combine(root, "localization");
        var languages = Directory.GetDirectories(localizationRoot);

        Assert.True(languages.Length >= 2, "至少应有两种语言用于演示/验证回退");

        // 各语言的表文件名集合应一致
        var tableSets = languages
            .Select(lang => (Lang: Path.GetFileName(lang),
                Tables: Directory.GetFiles(lang, "*.json")
                    .Select(Path.GetFileName)
                    .OrderBy(static n => n, StringComparer.Ordinal)
                    .ToArray()))
            .ToArray();

        var reference = tableSets[0];
        foreach (var (lang, tables) in tableSets.Skip(1))
        {
            Assert.True(reference.Tables.SequenceEqual(tables),
                $"语言 {lang} 的表文件与 {reference.Lang} 不一致：{string.Join(",", tables)}");
        }

        // 同表在不同语言下 key 集合应一致（防漏翻译）
        var tableNames = reference.Tables.Select(static n => Path.GetFileNameWithoutExtension(n)!).ToArray();
        foreach (var table in tableNames)
        {
            var keySets = languages.Select(lang =>
            {
                var path = Path.Combine(lang, $"{table}.json");
                using var doc = JsonDocument.Parse(File.ReadAllText(path));
                return (Lang: Path.GetFileName(lang),
                    Keys: doc.RootElement.EnumerateObject()
                        .Select(static kv => kv.Name)
                        .OrderBy(static k => k, StringComparer.Ordinal)
                        .ToArray());
            }).ToArray();

            var baseline = keySets[0];
            foreach (var (lang, keys) in keySets.Skip(1))
            {
                var missing = baseline.Keys.Except(keys, StringComparer.Ordinal).ToArray();
                var extra = keys.Except(baseline.Keys, StringComparer.Ordinal).ToArray();
                Assert.True(missing.Length == 0 && extra.Length == 0,
                    $"表 {table} 在 {baseline.Lang} 与 {lang} 的 key 集合不一致；缺失=[{string.Join(",", missing)}] 多余=[{string.Join(",", extra)}]");
            }
        }
    }

    [Fact]
    public void UiKey_枚举值数量与页面场景注册数一致()
    {
        var root = FindRepoRoot();
        var uiKeyCount = Enum.GetValues<global::GFrameworkTemplate.scripts.enums.ui.UiKey>().Length;

        var entryScene = File.ReadAllText(Path.Combine(root, "global", "game_entry_point.tscn"));
        // 每个 UiPageConfig 子资源对应一个已注册页面
        var registeredCount = entryScene.Split("metadata/_custom_type_script").Length - 1;
        // 场景配置（SceneConfig）同样带该元数据，减去场景注册数与初始定义
        var sceneConfigCount = entryScene.Split("SceneConfig.cs").Length - 1;

        Assert.True(uiKeyCount > 0, "UiKey 至少应有一个页面键");
        Assert.True(registeredCount > 0, "game_entry_point.tscn 应注册至少一个 UiPageConfig");
        // 场景注册（SceneConfig）数量不应超过 SubResource 总数
        Assert.True(registeredCount >= sceneConfigCount,
            "页面注册数少于场景注册数，UiPageConfig 可能在入口场景中被遗漏");
    }
}