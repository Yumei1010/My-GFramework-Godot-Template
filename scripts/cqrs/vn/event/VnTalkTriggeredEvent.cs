namespace GFrameworkTemplate.scripts.cqrs.vn.@event;

/// <summary>对话命令触发</summary>
public sealed class VnTalkTriggeredEvent
{
    public string? Talker { get; init; }
    public required string Content { get; init; }
    public bool IsCenter { get; init; }
    public string? AvatarPath { get; init; }
}
