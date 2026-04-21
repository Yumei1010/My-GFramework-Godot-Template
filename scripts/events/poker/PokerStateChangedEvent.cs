using GFrameworkGodotTemplate.scripts.enums.poker;
using GFrameworkGodotTemplate.scripts.poker;

namespace GFrameworkGodotTemplate.scripts.events.poker;

/// <summary>
/// 状态机状态变更事件类
/// 用于表示状态机状态发生变化的事件
/// </summary>
public class PokerStateChangedEvent
{
    /// <summary>
    ///     目标状态 <see cref="StateType"/>
    /// </summary>
    public required StateType State { get; init; }
    
    /// <summary>
    ///     响应事件的扑克 <see cref="IPoker"/> 实例
    /// </summary>
    public required IPoker Poker { get; init; }
}