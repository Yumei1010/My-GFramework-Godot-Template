using GFramework.Core.model;
using TimeToTwentyfour.scripts.enums.deck;

namespace TimeToTwentyfour.scripts.model.deck;
public partial class DeckModel : AbstractModel
{
    public List<Guid> Pokers { get; set; } = [];
    public DeckSortMode CurrentSortMode { get; set; } = DeckSortMode.Value;

    protected override void OnInit()
    {

    }
}