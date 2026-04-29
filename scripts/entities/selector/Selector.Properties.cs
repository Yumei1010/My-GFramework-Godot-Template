using TimeToTwentyfour.scripts.entities.poker;

namespace TimeToTwentyfour.scripts.entities.selector;

public partial class Selector
{
    public IList<IPoker> Selects => _selected;
    public int Count => _selected.Count;
    public int Capacity { get; set; }
    
    private readonly List<IPoker> _selected = [];
}