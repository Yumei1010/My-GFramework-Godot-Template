using TimeToTwentyfour.scripts.enums.poker;

namespace TimeToTwentyfour.scripts.entities.poker;

public partial class Poker
{
    public Guid Id { get; set; } = Guid.NewGuid();

    private PokerSuitType _suitType = PokerSuitType.Heart;
    private string _numValue = "24";
    private static PokerNumType DetectNumType(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return PokerNumType.Integer;
        var trimmed = value.Trim();
        if (trimmed.Contains('/')) return PokerNumType.Fraction;
        if (trimmed.Contains('.')) return PokerNumType.Decimal;
        return PokerNumType.Integer;
    }
}