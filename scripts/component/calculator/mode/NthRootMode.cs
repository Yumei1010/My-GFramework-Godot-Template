using TimeToTwentyfour.scripts.entities.poker;
using TimeToTwentyfour.scripts.enums.calculator;

namespace TimeToTwentyfour.scripts.component.calculator.mode;

/// <summary>
///     开 N 次方运算模式，对两手牌执行 a 开 b 次方。
/// </summary>
public sealed class NthRootMode : Mode
{
    public override ModeType ModeType => ModeType.NthRoot;
    public override bool IsBinary => true;

    public override string Calculate(IPokerData pokerA, IPokerData pokerB)
    {
        var fa = ParseToFraction(pokerA);
        var fb = ParseToFraction(pokerB);
        return fa.ToDouble() == 0 ? "ERROR:ZeroRootIndex" : FormatFractionResult(Math.Pow(fb.ToDouble(), 1.0 / fa.ToDouble()));
    }
}
