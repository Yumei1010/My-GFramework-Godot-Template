using GFrameworkTemplate.scripts.core.story;

namespace GFrameworkTemplate.scripts.cqrs.vn.@event;

/// <summary>分支选项触发</summary>
public sealed class VnBranchTriggeredEvent
{
    public required Dictionary<string, BranchOption> Options { get; init; }
}
