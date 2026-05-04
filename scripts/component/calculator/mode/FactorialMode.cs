using TimeToTwentyfour.scripts.entities.poker;
using TimeToTwentyfour.scripts.enums.calculator;

namespace TimeToTwentyfour.scripts.component.calculator.mode;

public class FactorialMode : Mode
{
    public override ModeType ModeType => ModeType.Factorial;
    public override bool IsBinary => false;

    public override string Calculate(IPoker poker)
    {
        double val = ParseToFraction(poker).ToDouble();
        return FormatDouble(CalculateFactorial(val));
    }
}
