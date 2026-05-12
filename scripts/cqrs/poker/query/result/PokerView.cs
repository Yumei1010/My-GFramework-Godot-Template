using TimeToTwentyfour.scripts.data.poker;

namespace cqrs.poker.query.result;

public sealed class PokerView
{
    public PokerData Poker { get; init; } = new();
}