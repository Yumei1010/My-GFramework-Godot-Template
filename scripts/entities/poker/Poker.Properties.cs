using TimeToTwentyfour.scripts.enums.poker;

namespace TimeToTwentyfour.scripts.entities.poker;

public partial class Poker
{
    public Guid Id { get; set; } = Guid.NewGuid();

    private SuitType _suitType = SuitType.Heart;
    private string _numValue = "24";
    private static NumType DetectNumType(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return NumType.Integer;
        var trimmed = value.Trim();
        if (trimmed.Contains('/')) return NumType.Fraction;
        if (trimmed.Contains('.')) return NumType.Decimal;
        return NumType.Integer;
    }
}