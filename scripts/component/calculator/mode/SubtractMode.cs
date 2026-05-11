using TimeToTwentyfour.scripts.model.poker;
using TimeToTwentyfour.scripts.enums.calculator;

namespace TimeToTwentyfour.scripts.component.calculator.mode;

/// <summary>
///     减法运算模式，对两手牌执行 a - b。
/// </summary>
public sealed class SubtractMode : Mode
{
    public override ModeType ModeType => ModeType.Subtract;
    public override bool IsBinary => true;

    public override string Calculate(IPokerData pokerA, IPokerData pokerB)
    {
        var fa = ParseToFraction(pokerA);
        var fb = ParseToFraction(pokerB);
        return (fa - fb).ToString();
    }
}
