using TimeToTwentyfour.scripts.entities.poker;

namespace TimeToTwentyfour.scripts.utility;

/// <summary>
///     Guid ↔ <see cref="IPokerView"/> 查找表实现。
/// </summary>
public class PokerRegistry : IPokerRegistry
{
    private readonly Dictionary<Guid, IPokerView> _map = new();

    /// <inheritdoc />
    public void Register(Guid id, IPokerView poker) => _map[id] = poker;

    /// <inheritdoc />
    public void Unregister(Guid id) => _map.Remove(id);

    /// <inheritdoc />
    public IPokerView? Find(Guid id) => _map.GetValueOrDefault(id);
}
