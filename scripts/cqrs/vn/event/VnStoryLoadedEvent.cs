namespace GFrameworkTemplate.scripts.cqrs.vn.@event;

/// <summary>故事脚本加载完成</summary>
public sealed class VnStoryLoadedEvent
{
    public required int CommandCount { get; init; }
}
