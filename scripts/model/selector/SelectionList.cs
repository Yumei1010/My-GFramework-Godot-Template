using TimeToTwentyfour.scripts.entities.poker;

namespace TimeToTwentyfour.scripts.component.selector;

/// <summary>
///     选择列表 —— 管理扑克选中状态的纯 C# 集合，不依赖 Godot/GFramework。
///     入队采用 FIFO（队首淘汰），弹出采用 LIFO（队尾提取）。
/// </summary>
public class SelectionList
{
    /// <summary>当前选中列表（只读），顺序为选中先后。</summary>
    public IReadOnlyList<IPoker> Items => _items;

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

    /// <summary>检查指定牌是否在列表中。</summary>
    public bool Contains(IPoker poker)
    {
        return _items.Contains(poker);
    }

    /// <summary>
    ///     将牌加入队尾。去重：已在列表中则忽略。若容量超限则驱逐队首。
    /// </summary>
    /// <returns>是否实际添加（去重导致忽略时返回 <c>false</c>）。</returns>
    public bool Add(IPoker poker)
    {
        if (_items.Contains(poker)) return false;

        if (_capacity > 0 && _items.Count >= _capacity)
        {
            _items.RemoveAt(0);
        }

        _items.Add(poker);
        return true;
    }

    /// <summary>从列表中移除指定牌。</summary>
    /// <returns>是否成功移除（牌不在列表中时返回 <c>false</c>）。</returns>
    public bool Remove(IPoker poker)
    {
        return _items.Remove(poker);
    }

    /// <summary>
    ///     LIFO 弹出最近加入的牌。列表为空时返回 <c>null</c>。
    /// </summary>
    public IPoker Pop()
    {
        if (_items.Count == 0) return null!;

        var lastIndex = _items.Count - 1;
        var poker = _items[lastIndex];
        _items.RemoveAt(lastIndex);
        return poker;
    }

    /// <summary>
    ///     容量缩小后，从队首淘汰超出上限的牌。
    /// </summary>
    public void TrimExcess()
    {
        while (_capacity > 0 && _items.Count > _capacity)
        {
            _items.RemoveAt(0);
        }
    }

    private readonly List<IPoker> _items = [];

    private int _capacity;
}
