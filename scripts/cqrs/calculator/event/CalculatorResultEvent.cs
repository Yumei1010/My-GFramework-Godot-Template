using TimeToTwentyfour.scripts.entities.poker;
using TimeToTwentyfour.scripts.enums.calculator;

namespace TimeToTwentyfour.scripts.cqrs.calculator.@event;

/// <summary>
///     计算结果事件类，用于传递运算结果及参与运算的手牌信息。
/// </summary>
public sealed class CalculatorResultEvent
{
    /// <summary>
    ///     计算结果字符串，以 "ERROR:" 开头表示错误。
    /// </summary>
    public required string Result { get; init; }
    /// <summary>
    ///     参与运算的手牌列表。
    /// </summary>
    public required IReadOnlyList<IPoker> Hands { get; init; }
    /// <summary>
    ///     使用的运算模式类型，运算未执行时为 null。
    /// </summary>
    public required ModeType? ModeType { get; init; }
    /// <summary>
    ///     是否为错误结果。
    /// </summary>
    public bool IsError => Result.StartsWith("ERROR:", StringComparison.Ordinal);
}
