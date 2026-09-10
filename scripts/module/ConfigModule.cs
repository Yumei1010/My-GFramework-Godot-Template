using GFramework.Core.Abstractions.Architectures;
using GFramework.Game.Abstractions.Config;
using GFramework.Game.Config;
using GFramework.Game.Config.Generated;
using GFramework.Godot.Config;
using GFrameworkTemplate.scripts.framework.config;

namespace GFrameworkTemplate.scripts.module;

/// <summary>
///     配置模块类：安装 GFramework Config 系统（YAML + JSON Schema + Source Generator 全流程演示）。
/// </summary>
/// <remarks>
///     使用 Godot 桥接加载器（<see cref="GodotYamlConfigLoader" />）：
///     编辑器态直接读 <c>res://</c>，导出态自动同步 YAML/schema 到 <c>user://config_cache</c>。
///     生成器约定：schemas 目录的 <c>*.schema.json</c> 驱动生成强类型配置类与表包装（命名空间 <c>GFramework.Game.Config.Generated</c>）。
/// </remarks>
public sealed class ConfigModule : IArchitectureModule
{
    /// <summary>
    ///     加载完成后供业务层读取的配置注册表。
    /// </summary>
    public IConfigRegistry? Registry { get; private set; }

    /// <summary>
    ///     安装配置系统：注册 Godot 桥接加载器，注册全部已生成配置表并执行加载。
    /// </summary>
    /// <param name="architecture">目标架构。</param>
    public void Install(IArchitecture architecture)
    {
        // 由生成器元数据自动给出表清单，避免手写表名/路径
        var tableSources = GeneratedConfigCatalog.Tables
            .Select(static metadata => new GodotYamlConfigTableSource(
                metadata.TableName,
                metadata.ConfigRelativePath,
                metadata.SchemaRelativePath))
            .ToArray();

        // Godot 桥接加载器：res:// 源 + user:// 缓存 + 聚合注册全部生成表
        var loader = new GodotYamlConfigLoader(new GodotYamlConfigLoaderOptions
        {
            SourceRootPath = "res://",
            RuntimeCacheRootPath = "user://config_cache",
            TableSources = tableSources,
            ConfigureLoader = static yamlLoader => yamlLoader.RegisterAllGeneratedConfigTables()
        });

        var registry = new ConfigRegistry();
        loader.LoadAsync(registry).GetAwaiter().GetResult();
        Registry = registry;

        // 注册进架构供业务层注入（GetUtility<IConfigRegistry>）
        architecture.RegisterUtility(registry);
        architecture.RegisterUtility(new ConfigRuntime(registry));
    }
}