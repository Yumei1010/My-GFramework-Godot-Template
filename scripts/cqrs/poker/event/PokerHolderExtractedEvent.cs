using TimeToTwentyfour.scripts.entities.poker;

namespace TimeToTwentyfour.scripts.cqrs.poker.@event;

/// <summary>
///     卡套被抽取事件类
///     用于表示卡套被抽取的事件
/// </summary>
public class PokerHolderExtractedEvent
{
    /// <summary>
    ///     抽出的扑克 <see cref="IPoker"/>
    /// </summary>
    public required IPoker Poker { get; init; }
}