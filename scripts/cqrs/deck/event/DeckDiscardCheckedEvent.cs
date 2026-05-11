namespace TimeToTwentyfour.scripts.cqrs.deck.@event;

/// <summary>
///     牌桌弃牌确认事件类
///     用于表示牌桌弃牌确认的事件
/// </summary>
public sealed class DeckDiscardCheckedEvent
{
    /// <summary>
    ///     要丢弃的手牌 Id 列表。
    /// </summary>
    public required IReadOnlyList<Guid> HandIds { get; init; }
}