using GFrameworkGodotTemplate.scripts.enums.poker;
using GFrameworkGodotTemplate.scripts.poker;

namespace GFrameworkGodotTemplate.scripts.events.poker;

/// <summary>
/// 扑克花色类型变更事件类
/// 用于表示扑克花色类型发生变化的事件
/// </summary>
public abstract class SuitTypeChangedEvent
{
    /// <summary>
    /// 花色类型
    /// </summary>
    public SuitType SuitType { get; set; }

    /// <summary>
    /// 响应事件的poker实例
    /// </summary>
    public Poker Poker { get; set; } = null!;
}