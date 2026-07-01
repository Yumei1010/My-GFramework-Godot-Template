namespace GFrameworkTemplate.scripts.cqrs.vn.@event;

/// <summary>跳转到另一个脚本</summary>
public sealed class VnGotoTriggeredEvent
{
    public required string TargetFilePath { get; init; }
}
