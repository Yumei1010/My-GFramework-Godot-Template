using TimeToTwentyfour.scripts.entities.poker;
using TimeToTwentyfour.scripts.enums.calculator;

namespace TimeToTwentyfour.scripts.component.calculator.mode;

/// <summary>
///     幂运算模式，对两手牌执行 a ^ b。
/// </summary>
public sealed class PowerMode : Mode
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
