using TimeToTwentyfour.scripts.entities.poker;
using TimeToTwentyfour.scripts.enums.calculator;

namespace TimeToTwentyfour.scripts.component.calculator.mode;

public class SubtractMode : Mode
{
    public override ModeType ModeType => ModeType.Subtract;
    public override bool IsBinary => true;

    public override string Calculate(IPoker pokerA, IPoker pokerB)
    {
        var fa = ParseToFraction(pokerA);
        var fb = ParseToFraction(pokerB);
        return (fa - fb).ToString();
    }
}
