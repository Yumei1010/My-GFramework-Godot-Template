using TimeToTwentyfour.scripts.entities.poker;

namespace TimeToTwentyfour.scripts.cqrs.poker.@event;

/// <summary>
///     卡套被插入事件类
///     用于表示卡套被插入的事件
/// </summary>
public class PokerHolderInsertedEvent
{
    /// <summary>
    ///     插入的扑克 <see cref="IPoker"/>
    /// </summary>
    public required IPoker Poker { get; init; }
}