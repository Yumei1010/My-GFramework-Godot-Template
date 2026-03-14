using GFrameworkGodotTemplate.scripts.enums.poker;
using GFrameworkGodotTemplate.scripts.poker;

namespace GFrameworkGodotTemplate.scripts.events.poker;

/// <summary>
/// 扑克数值类型变更事件类
/// 用于表示扑克数值类型发生变化的事件
/// </summary>
public abstract class NumTypeChangedEvent
{
    /// <summary>
    /// 数值类型
    /// </summary>
    public NumType NumType { get; set; }

    /// <summary>
    /// 响应事件的poker实例
    /// </summary>
    public Poker Poker { get; set; } = null!;
}