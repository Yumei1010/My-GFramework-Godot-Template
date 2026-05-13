using TimeToTwentyfour.scripts.entities.poker;
using TimeToTwentyfour.scripts.enums.calculator;

namespace TimeToTwentyfour.scripts.component.calculator.mode;

/// <summary>
///     绝对值运算模式，对单张手牌执行 |a|。
/// </summary>
public sealed class AbsoluteValueMode : Mode
{
    public override ModeType ModeType => ModeType.AbsoluteValue;
    public override bool IsBinary => false;

    public override string Calculate(IPokerData poker)
    {
        double val = ParseToFraction(poker).ToDouble();
        return FormatFractionResult(Math.Abs(val));
    }
}
