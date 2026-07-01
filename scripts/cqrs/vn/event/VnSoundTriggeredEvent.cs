namespace GFrameworkTemplate.scripts.cqrs.vn.@event;

/// <summary>音频播放触发</summary>
public sealed class VnSoundTriggeredEvent
{
    public required string SoundType { get; init; }
    public required string FilePath { get; init; }
}
