using TimeToTwentyfour.scripts.entities.poker;
using TimeToTwentyfour.scripts.enums.calculator;

namespace TimeToTwentyfour.scripts.component.calculator.mode;

public class PowerMode : Mode
{
    public override ModeType ModeType => ModeType.Power;
    public override bool IsBinary => true;

    public override string Calculate(IPoker pokerA, IPoker pokerB)
    {
        var fa = ParseToFraction(pokerA);
        var fb = ParseToFraction(pokerB);
        return FormatDouble(Math.Pow(fa.ToDouble(), fb.ToDouble()));
    }
}
