using TimeToTwentyfour.scripts.entities.poker;
using TimeToTwentyfour.scripts.enums.calculator;

namespace TimeToTwentyfour.scripts.component.calculator.mode;

/// <summary>
///     向上取整运算模式，对单张手牌执行 ⌈a⌉。
/// </summary>
public sealed class CeilMode : Mode
{
    public override ModeType ModeType => ModeType.Ceil;
    public override bool IsBinary => false;

    public override string Calculate(IPokerData poker)
    {
        double val = ParseToFraction(poker).ToDouble();
        return FormatFractionResult(Math.Ceiling(val));
    }
}
