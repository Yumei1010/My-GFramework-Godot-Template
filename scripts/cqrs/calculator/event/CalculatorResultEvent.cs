using TimeToTwentyfour.scripts.entities.poker;
using TimeToTwentyfour.scripts.enums.calculator;

namespace TimeToTwentyfour.scripts.cqrs.calculator.@event;

public sealed class CalculatorResultEvent
{
    public required string Result { get; init; }
    public required IReadOnlyList<IPoker> Hands { get; init; }
    public required ModeType? ModeType { get; init; }
    public bool IsError => Result.StartsWith("ERROR:", StringComparison.Ordinal);
}
