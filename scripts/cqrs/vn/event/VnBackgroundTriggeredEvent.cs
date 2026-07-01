namespace GFrameworkTemplate.scripts.cqrs.vn.@event;

/// <summary>背景切换触发</summary>
public sealed class VnBackgroundTriggeredEvent
{
    public required string FilePath { get; init; }
    public bool WaitTweenEnd { get; init; }
    public float Delay { get; init; }
}
