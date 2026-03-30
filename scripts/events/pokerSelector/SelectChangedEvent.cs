using GFrameworkGodotTemplate.scripts.poker;

namespace GFrameworkGodotTemplate.scripts.events.pokerSelector;

/// <summary>
/// 扑克选择器选择变更事件类
/// 用于表示扑克选择器选择发生变化的事件
/// </summary>
public class SelectChangedEvent
{
    /// <summary>
    /// 响应事件的poker实例
    /// </summary>
    public Poker Poker { get; set; } = null!;
}