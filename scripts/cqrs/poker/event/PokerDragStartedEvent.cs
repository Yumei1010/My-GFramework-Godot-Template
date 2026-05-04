using TimeToTwentyfour.scripts.entities.poker;

namespace TimeToTwentyfour.scripts.cqrs.poker.@event;

/// <summary>
///     扑克开始拖拽事件类
///     用于表示扑克开始拖拽的事件 
/// </summary>
public sealed class PokerDragStartedEvent
{
    /// <summary>
    ///     响应事件的poker实例 <see cref="IPoker"/>
    /// </summary>
    public required IPoker Poker { get; init; }
}