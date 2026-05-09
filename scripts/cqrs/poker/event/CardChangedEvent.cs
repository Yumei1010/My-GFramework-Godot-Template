using TimeToTwentyfour.scripts.enums.poker;

namespace TimeToTwentyfour.scripts.cqrs.poker.@event;

/// <summary>
///     牌面数据变更事件
///     当模型中 <see cref="Cards"/> 被修改时触发，Poker 视图通过 Id 匹配来同步自身。
/// </summary>
public sealed class CardChangedEvent
{
    public required Guid Id { get; init; }
    public required SuitType SuitType { get; init; }
    public required string NumValue { get; init; }
    public required NumType NumType { get; init; }
}
