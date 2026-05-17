using Godot;
using TimeToTwentyfour.scripts.entities.poker;
using TimeToTwentyfour.scripts.enums.deck;

namespace TimeToTwentyfour.scripts.entities.deck;

public partial class Deck
{
    private Dictionary<Panel, IPokerView> Mapping { get; set; } = [];
    
    private DeckSortMode CurrentSortMode { get; set; }
}