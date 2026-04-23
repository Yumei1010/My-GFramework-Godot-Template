using TimeToTwentyfour.scripts.enums.poker;

namespace TimeToTwentyfour.scripts.cqrs.poker.@event;

/// <summary>
/// 扑克选择器模式变更事件类
/// 用于表示扑克选择器模式变更的事件
/// </summary>
public class PokerSelectorModeChangedEvent
{
    /// <summary>
    /// 扑克选择器模式 <see cref="ModeType"/>
    /// </summary>
    public required ModeType Mode { get; init; }
}