using GFramework.Core.Abstractions.Utility;
using GFramework.Game.Abstractions.Config;
using GFramework.Game.Config.Generated;

namespace GFrameworkTemplate.scripts.core.config;

/// <summary>
///     配置读取入口：把初始化后的配置注册表封装成强类型访问，业务层不接触字符串表名。
/// </summary>
/// <remarks>
///     由 <c>ConfigModule</c> 初始化后注入。演示 <c>Registry.GetDifficultyTable()</c> 强类型表查询。
/// </remarks>
public sealed class ConfigRuntime : IUtility
{
    private readonly IConfigRegistry _registry;

    /// <summary>
    ///     创建配置读取入口。
    /// </summary>
    /// <param name="registry">已加载完成的配置注册表。</param>
    public ConfigRuntime(IConfigRegistry registry)
    {
        _registry = registry;
    }

    /// <summary>
    ///     获取指定难度配置。
    /// </summary>
    /// <param name="difficultyId">难度主键（easy/normal/hard）。</param>
    /// <returns>强类型难度配置。</returns>
    public DifficultyConfig GetDifficulty(string difficultyId)
    {
        return _registry.GetDifficultyTable().Get(difficultyId);
    }

    /// <summary>
    ///     获取全部难度配置。
    /// </summary>
    /// <returns>难度配置集合。</returns>
    public IReadOnlyCollection<DifficultyConfig> GetAllDifficulties()
    {
        return _registry.GetDifficultyTable().All();
    }
}