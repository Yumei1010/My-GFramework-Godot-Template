using GFrameworkGodotTemplate.scripts.enums.poker;
using GFrameworkGodotTemplate.scripts.poker;

namespace GFrameworkGodotTemplate.scripts.events.pokerStateMachine;

/// <summary>
/// 状态机状态变更事件类
/// 用于表示状态机状态发生变化的事件
/// </summary>
public class PokerStateMachineStateChangedEvent
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