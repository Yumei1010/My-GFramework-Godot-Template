using TimeToTwentyfour.scripts.enums.calculator;
using TimeToTwentyfour.scripts.entities.poker;

namespace TimeToTwentyfour.scripts.component.calculator.mode;

/// <summary>
///     运算模式契约
///     定义二元与一元计算入口。
/// </summary>
public interface IMode
{
    /// <summary>
    ///     该模式对应的运算类型 <see cref="ModeType"/>。
    /// </summary>
    ModeType ModeType { get; }

    /// <summary>
    ///     是否为二元运算模式。二元模式需两张手牌，一元模式只需一张。
    /// </summary>
    bool IsBinary { get; }

    /// <summary>
    ///     执行二元运算。
    /// </summary>
    /// <param name="pokerA">手牌 A <see cref="IPokerData"/></param>
    /// <param name="pokerB">手牌 B <see cref="IPokerData"/></param>
    /// <returns>计算结果字符串，或错误标识</returns>
    string Calculate(IPokerData pokerA, IPokerData pokerB);

    /// <summary>
    ///     执行一元运算。
    /// </summary>
    /// <param name="poker">手牌 <see cref="IPokerData"/></param>
    /// <returns>计算结果字符串，或错误标识</returns>
    string Calculate(IPokerData poker);
}
