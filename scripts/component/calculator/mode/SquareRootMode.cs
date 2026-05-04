using TimeToTwentyfour.scripts.entities.poker;
using TimeToTwentyfour.scripts.enums.calculator;

namespace TimeToTwentyfour.scripts.component.calculator.mode;

public class SquareRootMode : Mode
{
    public override ModeType ModeType => ModeType.SquareRoot;
    public override bool IsBinary => false;

    public override string Calculate(IPoker poker)
    {
        double val = ParseToFraction(poker).ToDouble();
        return val < 0 ? "ERROR:InvalidSqrt" : FormatDouble(Math.Sqrt(val));
    }
}
