using TimeToTwentyfour.scripts.entities.poker;

namespace TimeToTwentyfour.scripts.cqrs.selector.@event;

/// <summary>
///     扑克选择器预览变更事件类
///     用于表示扑克选择器预览发生变化的事件
/// </summary>
public sealed class PokerSelectorReservesChangedEvent
{
    /// <summary>
    ///     是否被选择 <see cref="bool"/>
    /// </summary>
    public required bool IsSelected { get; init; }
    
    /// <summary>
    ///     响应事件的poker实例 <see cref="IPoker"/>
    /// </summary>
    public required IPoker Poker { get; init; }
}