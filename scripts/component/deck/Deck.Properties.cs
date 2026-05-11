using Godot;
using TimeToTwentyfour.scripts.entities.poker;

namespace TimeToTwentyfour.scripts.component.deck;

public partial class Deck
{
    private Dictionary<Panel, IPokerView> Mapping { get; set; } = new();
    
    private SortMode CurrentSortMode { get; set; } = SortMode.Manual;

    private enum SortMode
    {
        Manual,
        BySuit,
        ByRank
    }
}