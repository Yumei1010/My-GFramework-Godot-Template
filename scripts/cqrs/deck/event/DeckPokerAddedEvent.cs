namespace TimeToTwentyfour.scripts.cqrs.deck.@event;

public sealed class DeckPokerAddedEvent
{
    public required Guid PokerId { get; init; }
}