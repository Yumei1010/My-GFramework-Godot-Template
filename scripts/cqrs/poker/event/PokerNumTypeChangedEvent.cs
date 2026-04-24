using TimeToTwentyfour.scripts.entities.poker;
using TimeToTwentyfour.scripts.enums.poker;

namespace TimeToTwentyfour.scripts.cqrs.poker.@event;

/// <summary>
///     扑克数值类型变更事件类
///     用于表示扑克数值类型发生变化的事件
/// </summary>
public abstract class PokerNumTypeChangedEvent
{
    /// <summary>
    ///     数值类型 <see cref="NumType"/>
    /// </summary>
    public required NumType NumType { get; init; }

    /// <summary>
    ///     响应事件的poker实例 <see cref="IPoker"/>
    /// </summary>
    public required IPoker Poker { get; init; }
}