namespace TimeToTwentyfour.scripts.cqrs.poker.@event;

/// <summary>
///     扑克开始拖拽事件类
///     用于表示扑克开始拖拽的事件
/// </summary>
public sealed class PokerDragStartedEvent
{
    /// <summary>
    ///     响应事件的扑克 Id。
    /// </summary>
    public required Guid PokerId { get; init; }
}