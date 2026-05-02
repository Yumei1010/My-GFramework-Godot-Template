using Godot;

namespace TimeToTwentyfour.scripts.component.pokerFactory;

public partial class PokerFactory
{
    [Export] private PackedScene _pokerScene = GD.Load<PackedScene>("res://scenes/entities/poker/poker.tscn");
}