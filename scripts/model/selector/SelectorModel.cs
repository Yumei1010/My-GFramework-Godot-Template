using GFramework.Core.model;
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
