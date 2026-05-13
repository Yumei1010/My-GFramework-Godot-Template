using TimeToTwentyfour.scripts.enums.calculator;
using TimeToTwentyfour.scripts.entities.poker;

namespace TimeToTwentyfour.scripts.component.calculator;

/// <summary>
///     计算器契约，管理运算模式的切换与计算委托。
/// </summary>
public interface ICalculator
{
    /// <summary>
    ///     切换到指定运算模式。
    /// </summary>
    /// <param name="modeType">目标模式 <see cref="ModeType"/></param>
    void ChangeTo(ModeType modeType);

    /// <summary>
    ///     使用当前模式执行二元运算。
    /// </summary>
    /// <param name="pokerA">手牌 A <see cref="IPokerData"/></param>
    /// <param name="pokerB">手牌 B <see cref="IPokerData"/></param>
    /// <returns>计算结果字符串</returns>
    string Calculate(IPokerData pokerA, IPokerData pokerB);

    /// <summary>
    ///     使用当前模式执行一元运算。
    /// </summary>
    /// <param name="poker">手牌 <see cref="IPokerData"/></param>
    /// <returns>计算结果字符串</returns>
    string Calculate(IPokerData poker);

    /// <summary>
    ///     当前运算模式类型。未设置时为 <see langword="null"/>。
    /// </summary>
    ModeType CurrentModeType { get; }
}
