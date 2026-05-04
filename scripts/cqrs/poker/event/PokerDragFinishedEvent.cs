using TimeToTwentyfour.scripts.entities.poker;

namespace TimeToTwentyfour.scripts.cqrs.poker.@event;

/// <summary>
///     扑克拖拽结束事件类
///     用于表示扑克拖拽结束的事件 
/// </summary>
public sealed class PokerDragFinishedEvent
{
    /// <summary>
    ///     响应事件的poker实例 <see cref="IPoker"/>
    /// </summary>
    public required IPoker Poker { get; init; }
}