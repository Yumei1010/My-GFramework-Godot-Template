using TimeToTwentyfour.scripts.entities.poker;
using TimeToTwentyfour.scripts.enums.calculator;

namespace TimeToTwentyfour.scripts.component.calculator.mode;

/// <summary>
///     平方根运算模式，对单张手牌执行 √a。
/// </summary>
public sealed class SquareRootMode : Mode
{
    public override ModeType ModeType => ModeType.SquareRoot;
    public override bool IsBinary => false;

    public override string Calculate(IPokerData poker)
    {
        double val = ParseToFraction(poker).ToDouble();
        return val < 0 ? "ERROR:InvalidSqrt" : FormatFractionResult(Math.Sqrt(val));
    }
}
