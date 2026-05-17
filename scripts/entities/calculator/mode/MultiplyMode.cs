using TimeToTwentyfour.scripts.entities.poker;
using TimeToTwentyfour.scripts.enums.calculator;

namespace TimeToTwentyfour.scripts.entities.calculator.mode;

/// <summary>
///     乘法运算模式，对两手牌执行 a * b。
/// </summary>
public sealed class MultiplyMode : Mode
{
    public override ModeType ModeType => ModeType.Multiply;
    public override bool IsBinary => true;

    public override string Calculate(IPokerData pokerA, IPokerData pokerB)
    {
        var fa = ParseToFraction(pokerA);
        var fb = ParseToFraction(pokerB);
        return (fa * fb).ToString();
    }
}
