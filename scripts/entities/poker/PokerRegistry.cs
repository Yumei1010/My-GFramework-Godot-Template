namespace TimeToTwentyfour.scripts.entities.poker;

/// <summary>
///     Guid ↔ <see cref="IPokerView"/> 轻量映射表，供 CQRS 事件层通过 Id 查找视图节点。
/// </summary>
public static class PokerRegistry
{
    private static readonly Dictionary<Guid, IPokerView> _map = new();

    /// <summary>注册扑克视图节点到查找表。</summary>
    public static void Register(Guid id, IPokerView poker) => _map[id] = poker;

    /// <summary>从查找表移除扑克视图节点。</summary>
    public static void Unregister(Guid id) => _map.Remove(id);

    /// <summary>按 Id 查找扑克视图节点，未找到时返回 <see langword="null"/>。</summary>
    public static IPokerView? Find(Guid id) => _map.GetValueOrDefault(id);
}
