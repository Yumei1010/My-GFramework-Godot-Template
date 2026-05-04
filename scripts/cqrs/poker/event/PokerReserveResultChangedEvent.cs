using TimeToTwentyfour.scripts.entities.poker;

namespace TimeToTwentyfour.scripts.cqrs.poker.@event;

/// <summary>
///     扑克预览运算结果变更事件类
///     用于表示扑克预览运算结果发生变化的事件 
/// </summary>
public sealed class PokerReserveResultChangedEvent
{
    /// <summary>
    ///     点数数值 <see cref="string"/>
    /// </summary>
    public required string NumValue { get; init; }
    
    /// <summary>
    ///     是否被隐藏 <see cref="bool"/>
    /// </summary>
    public required bool IsHidden { get; init; }

    /// <summary>
    ///     响应事件的poker实例 <see cref="IPoker"/>
    /// </summary>
    public required IPoker Poker { get; init; }
}