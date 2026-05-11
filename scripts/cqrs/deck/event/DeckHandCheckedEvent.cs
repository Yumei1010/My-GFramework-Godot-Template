namespace TimeToTwentyfour.scripts.cqrs.deck.@event;

/// <summary>
///     牌桌出牌确认事件类
///     用于表示牌桌出牌确认的事件
/// </summary>
public sealed class DeckHandCheckedEvent
{
    /// <summary>
    ///     要打出的手牌 Id 列表。
    /// </summary>
    public required IReadOnlyList<Guid> HandIds { get; init; }
}