using GFramework.Core.extensions;
using TimeToTwentyfour.scripts.cqrs.selector.@event;
using TimeToTwentyfour.scripts.entities.poker;

namespace TimeToTwentyfour.scripts.component.selector;

public partial class Selector
{
    public IReadOnlyList<IPoker> Selects => _selected;
    public int Count => _selected.Count;
    public int Capacity
    {
        get => _capacity;
        set
        {
            _capacity = value;
            TrimExcess();
        }
    }
    
    private readonly List<IPoker> _selected = [];
    private int _capacity;

    /// <summary>
    ///     容量缩小后，从队首淘汰超出上限的牌。
    /// </summary>
    private void TrimExcess()
    {
        while (_capacity > 0 && _selected.Count > _capacity)
        {
            var evicted = _selected[0];
            _selected.RemoveAt(0);
            ContextAwareExtensions.SendEvent(this, new SelectorSelectChangedEvent { IsSelected = false, Poker = evicted });
        }
    }
}