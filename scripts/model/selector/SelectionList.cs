namespace TimeToTwentyfour.scripts.model.selector;

/// <summary>
///     选择列表 —— 管理扑克选中状态的纯 C# 集合，不依赖 Godot/GFramework。
///     入队采用 FIFO（队首淘汰），弹出采用 LIFO（队尾提取）。
///     存储元素为扑克 <see cref="Guid"/>，与视图层彻底解耦。
/// </summary>
public class SelectionList
{
    /// <summary>驱逐通知 —— 当 Id 因容量限制被移出列表时触发。</summary>
    public event Action<Guid>? Evicted;

    /// <summary>当前选中列表（只读），顺序为选中先后。</summary>
    public IReadOnlyList<Guid> Items => _items;

    /// <summary>当前选中数量。</summary>
    public int Count => _items.Count;

    /// <summary>
    ///     选择上限。<c>0</c> 表示无上限，设置更小值会自动触发 <see cref="TrimExcess"/>。
    /// </summary>
    public int Capacity
    {
        get => _capacity;
        set
        {
            _capacity = value;
            TrimExcess();
        }
    }

    /// <summary>检查指定 Id 是否在列表中。</summary>
    public bool Contains(Guid pokerId)
    {
        return _items.Contains(pokerId);
    }

    /// <summary>
    ///     将 Id 加入队尾。去重：已在列表中则忽略。若容量超限则驱逐队首。
    /// </summary>
    /// <returns>是否实际添加（去重导致忽略时返回 <c>false</c>）。</returns>
    public bool Add(Guid pokerId)
    {
        if (_items.Contains(pokerId)) return false;

        if (_capacity > 0 && _items.Count >= _capacity)
        {
            var evicted = _items[0];
            _items.RemoveAt(0);
            Evicted?.Invoke(evicted);
        }

        _items.Add(pokerId);
        return true;
    }

    /// <summary>从列表中移除指定 Id。</summary>
    /// <returns>是否成功移除（Id 不在列表中时返回 <c>false</c>）。</returns>
    public bool Remove(Guid pokerId)
    {
        return _items.Remove(pokerId);
    }

    /// <summary>
    ///     LIFO 弹出最近加入的 Id。列表为空时返回 <c>Guid.Empty</c>。
    /// </summary>
    public Guid Pop()
    {
        if (_items.Count == 0) return Guid.Empty;

        var lastIndex = _items.Count - 1;
        var pokerId = _items[lastIndex];
        _items.RemoveAt(lastIndex);
        return pokerId;
    }

    /// <summary>
    ///     容量缩小后，从队首淘汰超出上限的 Id。
    /// </summary>
    public void TrimExcess()
    {
        while (_capacity > 0 && _items.Count > _capacity)
        {
            var evicted = _items[0];
            _items.RemoveAt(0);
            Evicted?.Invoke(evicted);
        }
    }

    private readonly List<Guid> _items = [];

    private int _capacity;
}
