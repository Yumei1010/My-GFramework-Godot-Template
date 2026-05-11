using GFramework.Core.model;
using TimeToTwentyfour.scripts.component.selector;
using TimeToTwentyfour.scripts.entities.poker;

namespace TimeToTwentyfour.scripts.model.selector;

public class SelectorModel : AbstractModel
{
    public SelectionList Selects { 
        get => _selects; 
        set => _selects = value; 
    }

    public bool Enable { get; set; } = false;

    protected override void OnInit()
    {

    }

    private SelectionList _selects = null!;
}
