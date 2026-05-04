using TimeToTwentyfour.scripts.entities.poker;
using TimeToTwentyfour.scripts.enums.calculator;

namespace TimeToTwentyfour.scripts.component.calculator.mode;

/// <summary>
///     向下取整运算模式，对单张手牌执行 ⌊a⌋。
/// </summary>
public sealed class FloorMode : Mode
{
    public override ModeType ModeType => ModeType.Floor;
    public override bool IsBinary => false;

    public override string Calculate(IPoker poker)
    {
        double val = ParseToFraction(poker).ToDouble();
        return FormatDouble(Math.Floor(val));
    }
}
