namespace TimeToTwentyfour.scripts.cqrs.selector.@event;

/// <summary>
///     扑克选择器可用性变更事件类
///     用于表示扑克选择器可用性发生变化的事件
/// </summary>
public sealed class SelectorEnableChangedEvent
{
    /// <summary>
    ///     可用性 <see cref="bool"/>
    /// </summary>
    public required bool Enable { get; init; }
}