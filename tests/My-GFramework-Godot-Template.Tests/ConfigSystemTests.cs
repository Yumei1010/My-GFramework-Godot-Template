using System;
using System.Collections.Generic;
using System.IO;
using GFramework.Game.Config;
using GFramework.Game.Config.Generated;

namespace GFrameworkTemplate.Tests;

/// <summary>
///     GFramework Config 系统验证：加载本地 YAML + schema 校验 + 强类型读取。
///     使用非 Godot 的 YamlConfigLoader（普通文件系统路径），不依赖 Godot 运行时。
/// </summary>
public class ConfigSystemTests
{
    /// <summary>
    ///     计算仓库根目录（tests 项目在 tests/My-GFramework-Godot-Template.Tests/ 下两层）。
    /// </summary>
    private static string FindRepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is { Parent: not null })
        {
            if (Directory.Exists(Path.Combine(dir.FullName, "schemas")))
            {
                return dir.FullName;
            }

            dir = dir.Parent;
        }

        throw new DirectoryNotFoundException("schemas 目录未找到（仓库根）");
    }

    private static ConfigRegistry LoadRegistry()
    {
        var root = FindRepoRoot();
        // 加载器根 = 仓库根；表相对路径与 schema 相对路径都由生成器元数据给出
        var registry = new ConfigRegistry();
        var loader = new YamlConfigLoader(root)
            .RegisterTable<string, DifficultyConfig>(
                DifficultyConfigBindings.Metadata.TableName,
                DifficultyConfigBindings.Metadata.ConfigRelativePath,
                DifficultyConfigBindings.Metadata.SchemaRelativePath,
                static c => c.Id,
                StringComparer.Ordinal);
        loader.LoadAsync(registry).GetAwaiter().GetResult();
        return registry;
    }

    [Fact]
    public void 加载_三档难度全部读取()
    {
        var registry = LoadRegistry();
        var table = registry.GetDifficultyTable();

        Assert.Equal(3, table.Count);
        Assert.True(table.ContainsKey("easy"));
        Assert.True(table.ContainsKey("normal"));
        Assert.True(table.ContainsKey("hard"));
    }

    [Fact]
    public void 强类型读取_Normal配置正确()
    {
        var registry = LoadRegistry();
        var normal = registry.GetDifficultyTable().Get("normal");

        Assert.Equal(4, normal.CardCount);
        Assert.Equal(60, normal.TimeLimitSec);
        Assert.Equal("普通", normal.Name);
        Assert.True(normal.HintEnabled);
        Assert.Equal(4, normal.OperatorLimit);
    }

    [Fact]
    public void 强类型读取_Hard限制运算符且禁提示()
    {
        var registry = LoadRegistry();
        var hard = registry.GetDifficultyTable().Get("hard");

        Assert.Equal(6, hard.CardCount);
        Assert.False(hard.HintEnabled);
        Assert.Equal(3, hard.OperatorLimit);
    }

    [Fact]
    public void 生成元数据_表名与路径正确()
    {
        Assert.Equal("difficulty", DifficultyConfigBindings.Metadata.TableName);
        Assert.True(DifficultyConfigBindings.Metadata.SchemaRelativePath.EndsWith(
            "difficulty.schema.json", StringComparison.Ordinal));
    }

    [Fact]
    public void 目录_未声明配置Key时返回默认()
    {
        var registry = LoadRegistry();
        var table = registry.GetDifficultyTable();

        Assert.False(table.ContainsKey("extreme"));   // 不存在的档位无配置
    }
}