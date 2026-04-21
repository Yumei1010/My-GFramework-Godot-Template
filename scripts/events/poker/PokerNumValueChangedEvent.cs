using GFrameworkGodotTemplate.scripts.poker;

namespace GFrameworkGodotTemplate.scripts.events.poker;

/// <summary>
/// 扑克点数数值变更事件类
/// 用于表示扑克点数数值发生变化的事件
/// </summary>
public abstract class PokerNumValueChangedEvent
{
    /// <summary>
    ///     点数数值
    /// </summary>
    public required string NumValue { get; init; }

    /// <summary>
    ///     响应事件的poker实例
    /// </summary>
    public required IPoker Poker { get; init; }
}