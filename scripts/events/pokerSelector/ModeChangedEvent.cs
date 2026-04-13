namespace GFrameworkGodotTemplate.scripts.events.pokerSelector;

/// <summary>
/// 选择器模式变更事件类
/// 用于表示选择器模式变更的事件
/// </summary>
public class ModeChangedEvent
{
    /// <summary>
    /// 模式
    /// </summary>
    public required string Mode { get; init; }
}