using TimeToTwentyfour.scripts.entities.poker;
using TimeToTwentyfour.scripts.enums.calculator;

namespace TimeToTwentyfour.scripts.entities.calculator.mode;

/// <summary>
///     阶乘运算模式，对单张手牌执行 a!。
/// </summary>
public sealed class FactorialMode : Mode
{
    public override ModeType ModeType => ModeType.Factorial;
    public override bool IsBinary => false;

    public override string Calculate(IPokerData poker)
    {
        double val = ParseToFraction(poker).ToDouble();
        return FormatFractionResult(CalculateFactorial(val));
    }
}
