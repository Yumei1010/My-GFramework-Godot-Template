using TimeToTwentyfour.scripts.entities.poker;

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

    public bool Enable { get; set; } = true;

    private readonly SelectionList _selection = new();
}
