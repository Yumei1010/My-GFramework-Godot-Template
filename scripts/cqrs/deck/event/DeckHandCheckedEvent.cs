using TimeToTwentyfour.scripts.entities.poker;

namespace TimeToTwentyfour.scripts.cqrs.deck.@event;

/// <summary>
///     牌桌出牌确认事件类
///     用于表示牌桌出牌确认的事件 
/// </summary>
public class DeckHandCheckedEvent
{
    /// <summary>
    ///     要打出的手牌 <see cref="IReadOnlyList{IPoker}"/>
    /// </summary>
    public required IReadOnlyList<IPoker> Hands { get; init;}
}