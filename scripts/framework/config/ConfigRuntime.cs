using GFramework.Core.Abstractions.Utility;
using GFramework.Game.Abstractions.Config;
using GFramework.Game.Config.Generated;

namespace GFrameworkTemplate.scripts.framework.config;

/// <summary>
///     配置读取入口：把初始化后的配置注册表封装成强类型访问，业务层不接触字符串表名。
/// </summary>
/// <remarks>
///     由 <c>ConfigModule</c> 初始化后注册进架构。演示 <c>Registry.GetMonsterTable()</c> 强类型表查询。
///     示例域为 <c>config/monster</c>（教学样品）；开发自己的游戏时替换为对应配置域的表方法。
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
    ///     获取底层配置注册表。
    /// </summary>
    /// <remarks>
    ///     可直接使用源生成器提供的强类型表扩展（如 <c>Registry.GetMonsterTable()</c>），
    ///     便于在不新增 ConfigRuntime 方法的情况下访问自定义配置域。
    /// </remarks>
    public IConfigRegistry Registry => _registry;

    /// <summary>
    ///     获取指定怪物配置（示例：config/monster 表）。
    /// </summary>
    /// <param name="monsterId">怪物主键（slime/goblin/bat/orc）。</param>
    /// <returns>强类型怪物配置。</returns>
    public MonsterConfig GetMonster(string monsterId)
    {
        return _registry.GetMonsterTable().Get(monsterId);
    }

    /// <summary>
    ///     获取全部怪物配置。
    /// </summary>
    /// <returns>怪物配置集合。</returns>
    public IReadOnlyCollection<MonsterConfig> GetAllMonsters()
    {
        return _registry.GetMonsterTable().All();
    }
}