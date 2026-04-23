using TimeToTwentyfour.scripts.entities.poker;
using TimeToTwentyfour.scripts.enums.poker;

namespace TimeToTwentyfour.scripts.cqrs.poker.@event;

/// <summary>
/// 扑克花色类型变更事件类
/// 用于表示扑克花色类型发生变化的事件
/// </summary>
public abstract class PokerSuitTypeChangedEvent
{
    /// <summary>
    ///     花色类型 <see cref="SuitType"/>
    /// </summary>
    public required SuitType SuitType { get; init; }

    /// <summary>
    ///     响应事件的poker实例 <see cref="IPoker"/>
    /// </summary>
    public required IPoker Poker { get; init; }
}