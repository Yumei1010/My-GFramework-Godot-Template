namespace GFrameworkTemplate.scripts.cqrs.vn.@event;

/// <summary>文本显示进度（打字机效果中）</summary>
public sealed class VnTextRevealProgressEvent
{
    public required int RevealedChars { get; init; }
    public required int TotalChars { get; init; }
}
