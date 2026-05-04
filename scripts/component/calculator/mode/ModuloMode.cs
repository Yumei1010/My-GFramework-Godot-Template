using TimeToTwentyfour.scripts.entities.poker;
using TimeToTwentyfour.scripts.enums.calculator;

namespace TimeToTwentyfour.scripts.component.calculator.mode;

public class ModuloMode : Mode
{
    public override ModeType ModeType => ModeType.Modulo;
    public override bool IsBinary => true;

    public override string Calculate(IPoker pokerA, IPoker pokerB)
    {
        var fa = ParseToFraction(pokerA);
        var fb = ParseToFraction(pokerB);
        return fb.ToDouble() == 0 ? "ERROR:DivByZero" : FormatDouble(fa.ToDouble() % fb.ToDouble());
    }
}
