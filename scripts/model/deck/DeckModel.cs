using GFramework.Core.model;
using Godot;
using TimeToTwentyfour.scripts.entities.poker;
using TimeToTwentyfour.scripts.enums.deck;

namespace TimeToTwentyfour.scripts.model.deck;
public class DeckModel : AbstractModel
{
    public List<Guid> Pokers { get; set; } = [];
    public DeckSortMode CurrentSortMode { get; set; } = DeckSortMode.Value;
    public Dictionary<Panel, IPokerView> Mapping { get; set; } = [];

    protected override void OnInit()
    {

    }
}