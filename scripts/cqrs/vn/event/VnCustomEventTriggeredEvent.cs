namespace GFrameworkTemplate.scripts.cqrs.vn.@event;

/// <summary>自定义事件触发——章节特定逻辑</summary>
public sealed class VnCustomEventTriggeredEvent
{
    public required string EventName { get; init; }
}
