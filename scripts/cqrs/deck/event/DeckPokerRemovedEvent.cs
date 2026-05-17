namespace TimeToTwentyfour.scripts.cqrs.deck.@event;

public sealed class DeckPokerRemovedEvent
{
    public required Guid PokerId { get; init; }
}