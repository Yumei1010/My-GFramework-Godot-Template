using GFrameworkTemplate.scripts.core.story;

namespace GFrameworkTemplate.scripts.cqrs.vn.@event;

/// <summary>立绘操作触发</summary>
public sealed class VnTachieTriggeredEvent
{
    public required Dictionary<string, TachieSlot> Tachies { get; init; }
}
