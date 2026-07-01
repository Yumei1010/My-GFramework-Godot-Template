namespace GFrameworkTemplate.scripts.cqrs.vn.@event;

/// <summary>玩家选中了某个分支选项</summary>
public sealed class VnBranchChosenEvent
{
    public required string OptionId { get; init; }
}
