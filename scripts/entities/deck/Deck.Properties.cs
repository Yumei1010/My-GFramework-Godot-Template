using Godot;
using TimeToTwentyfour.scripts.entities.poker;

namespace TimeToTwentyfour.scripts.entities.deck;

public partial class Deck
{
    private Dictionary<Panel, IPoker> Mapping { get; set; } = new();
    
    private SortMode CurrentSortMode { get; set; } = SortMode.Manual;

    private enum SortMode
    {
        Manual,
        BySuit,
        ByRank
    }
}