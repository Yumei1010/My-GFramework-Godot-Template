using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;

namespace GFrameworkGodotTemplate.scripts.poker;

/// <summary>
/// 扑克工厂类，负责根据定义创建扑克实例
/// </summary>
[Log]
[ContextAware]

public partial class PokerFactory
{
    private readonly Godot.Collections.Dictionary<int, PokerDefinition> _defs;

    /// <summary>
    /// 初始化扑克工厂实例
    /// </summary>
    /// <param name="defs">包含扑克定义数据的数据对象</param>
    public PokerFactory(Godot.Collections.Dictionary<int, PokerDefinition> defs)
    {
        // 将扑克定义数据转换为以ID为键的字典，便于快速查找
        _defs = defs;
    }

    /// <summary>
    /// 根据指定的扑克ID配置空间岩石对象
    /// </summary>
    /// <param name="poker">需要被配置的扑克对象</param>
    /// <param name="asteroidId">用于查找扑克定义的扑克ID</param>
    public void Configure(Poker poker, int asteroidId)
    {
        // 根据ID从字典中获取对应的扑克定义
        var def = _defs[asteroidId];

        // 使用扑克定义中的属性初始化扑克对象
        poker.Init(def);
    }
}