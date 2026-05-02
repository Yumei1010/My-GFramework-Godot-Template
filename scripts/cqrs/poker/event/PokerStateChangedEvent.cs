using TimeToTwentyfour.scripts.entities.poker;
using TimeToTwentyfour.scripts.enums.poker;

namespace TimeToTwentyfour.scripts.cqrs.poker.@event;

/// <summary>
///     扑克状态变更事件类
///     用于表示扑克状态发生变化的事件
/// </summary>
public class PokerStateChangedEvent
{
    /// <summary>
    ///     目标状态 <see cref="StateType"/>
    /// </summary>
    public required StateType TargetState { get; init; }
    
    /// <summary>
    ///     响应事件的扑克状态 <see cref="IPokerState"/> 实例
    /// </summary>
    public required IPokerState State { get; init; }
}