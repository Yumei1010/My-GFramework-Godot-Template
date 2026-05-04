using TimeToTwentyfour.scripts.entities.poker;
using TimeToTwentyfour.scripts.enums.calculator;

namespace TimeToTwentyfour.scripts.component.calculator.mode;

public class NthRootMode : Mode
{
    public override ModeType ModeType => ModeType.NthRoot;
    public override bool IsBinary => true;

    public override string Calculate(IPoker pokerA, IPoker pokerB)
    {
        var fa = ParseToFraction(pokerA);
        var fb = ParseToFraction(pokerB);
        return fa.ToDouble() == 0 ? "ERROR:ZeroRootIndex" : FormatDouble(Math.Pow(fb.ToDouble(), 1.0 / fa.ToDouble()));
    }
}
