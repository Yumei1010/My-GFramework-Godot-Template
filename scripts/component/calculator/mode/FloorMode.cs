using TimeToTwentyfour.scripts.entities.poker;
using TimeToTwentyfour.scripts.enums.calculator;

namespace TimeToTwentyfour.scripts.component.calculator.mode;

public class FloorMode : Mode
{
    public override ModeType ModeType => ModeType.Floor;
    public override bool IsBinary => false;

    public override string Calculate(IPoker poker)
    {
        double val = ParseToFraction(poker).ToDouble();
        return FormatDouble(Math.Floor(val));
    }
}
