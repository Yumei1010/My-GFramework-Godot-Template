using GFramework.Core.Abstractions.command;

namespace TimeToTwentyfour.scripts.cqrs.selector.@event;

/// <summary>
///     扑克选择器可用性变更事件类
///     用于表示扑克选择器可用性发生变化的事件
/// </summary>
public class SelectorEnableChangedEvent
{
    /// <summary>
    ///     可用性 <see cref="bool"/>
    /// </summary>
    public bool Enable { get; init; }
}