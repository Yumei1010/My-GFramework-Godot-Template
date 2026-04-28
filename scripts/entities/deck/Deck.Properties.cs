using Godot;
using TimeToTwentyfour.scripts.entities.poker;

namespace TimeToTwentyfour.scripts.entities.deck;

public partial class Deck
{
    [ExportGroup("Debug")] 
        [Export] private bool Holder { get; set; } = true;
    
    /// <summary>
    ///     卡册 <see cref="Dictionary{IPokerHolder, IPoker}"/>
    /// </summary>
    private Dictionary<Panel, IPoker> Album { get; set; } = new();
 
    private class MappingData
    {
        public Vector2 Position { get; set; }
        public float Angle { get; set; }
    }
    
    private Dictionary<int, MappingData> Mapping { get; set; } = new();
}