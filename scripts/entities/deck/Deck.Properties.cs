using Godot;
using TimeToTwentyfour.scripts.entities.poker;

namespace TimeToTwentyfour.scripts.entities.deck;

public partial class Deck
{
    private Dictionary<Panel, IPokerView> Mapping { get; set; } = [];
}