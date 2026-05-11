using GFramework.Core.extensions;
using TimeToTwentyfour.scripts.entities.poker;
using TimeToTwentyfour.scripts.model.selector;

namespace TimeToTwentyfour.scripts.component.selector;

public partial class Selector
{
    public IReadOnlyList<IPoker> Selects => _selection.Items;
    
    public int Count => _selection.Count;

    public int Capacity
    {
        get => _selection.Capacity;
        set => _selection.Capacity = value;
    }
}
