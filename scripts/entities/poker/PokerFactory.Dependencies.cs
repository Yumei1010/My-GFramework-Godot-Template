using Godot;

namespace TimeToTwentyfour.scripts.entities.poker;

public partial class PokerFactory
{
    [Export] private PackedScene _pokerScene = GD.Load<PackedScene>("res://scenes/entities/poker/poker.tscn");
}