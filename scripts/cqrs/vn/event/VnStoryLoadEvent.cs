namespace GFrameworkTemplate.scripts.cqrs.vn.@event;

/// <summary>开始加载故事脚本</summary>
public sealed class VnStoryLoadEvent
{
    public required string FilePath { get; init; }
}
