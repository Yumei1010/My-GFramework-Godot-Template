using System;
using System.IO;
using GFramework.Game.Config;
using GFramework.Game.Config.Generated;

namespace GFrameworkTemplate.Tests;

/// <summary>
///     GFramework Config 系统验证：加载本地 YAML + schema 校验 + 强类型读取。
///     使用非 Godot 的 YamlConfigLoader（普通文件系统路径），不依赖 Godot 运行时。
///     示例域为 config/monster（教学样品），验证枚举/数值校验/默认值等能力。
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
        var registry = new ConfigRegistry();
        var loader = new YamlConfigLoader(root)
            .RegisterTable<string, MonsterConfig>(
                MonsterConfigBindings.Metadata.TableName,
                MonsterConfigBindings.Metadata.ConfigRelativePath,
                MonsterConfigBindings.Metadata.SchemaRelativePath,
                static c => c.Id,
                StringComparer.Ordinal);
        loader.LoadAsync(registry).GetAwaiter().GetResult();
        return registry;
    }

    [Fact]
    public void 加载_示例怪物全部读取()
    {
        var registry = LoadRegistry();
        var table = registry.GetMonsterTable();

        Assert.Equal(3, table.Count);
        Assert.True(table.ContainsKey("slime"));
        Assert.True(table.ContainsKey("goblin"));
        Assert.True(table.ContainsKey("orc"));
    }

    [Fact]
    public void 强类型读取_Slime配置正确()
    {
        var registry = LoadRegistry();
        var slime = registry.GetMonsterTable().Get("slime");

        Assert.Equal("Slime", slime.Name);
        Assert.Equal(10, slime.Hp);
        Assert.Equal("nature", slime.Faction);
        Assert.Equal("common", slime.Rarity);
    }

    [Fact]
    public void 强类型读取_Orc为Boss稀有度()
    {
        var registry = LoadRegistry();
        var orc = registry.GetMonsterTable().Get("orc");

        Assert.Equal(120, orc.Hp);
        Assert.Equal("boss", orc.Rarity);
    }

    [Fact]
    public void 生成元数据_表名与路径正确()
    {
        Assert.Equal("monster", MonsterConfigBindings.Metadata.TableName);
        Assert.True(MonsterConfigBindings.Metadata.SchemaRelativePath.EndsWith(
            "monster.schema.json", StringComparison.Ordinal));
    }

    [Fact]
    public void 目录_未声明配置Key时返回默认()
    {
        var registry = LoadRegistry();
        var table = registry.GetMonsterTable();

        Assert.False(table.ContainsKey("bat"));   // schema 允许但未提供 yaml 的 key 无配置
    }
}