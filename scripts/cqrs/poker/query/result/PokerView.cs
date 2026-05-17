using TimeToTwentyfour.scripts.data.poker;

namespace TimeToTwentyfour.scripts.cqrs.poker.query.result;

public sealed class PokerView
{
    public PokerData Poker { get; init; } = new();
}