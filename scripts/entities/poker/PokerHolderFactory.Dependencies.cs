using Godot;

namespace TimeToTwentyfour.scripts.entities.poker;

public partial class PokerHolderFactory
{
    [Export] private PackedScene _pokerHolderScene = GD.Load<PackedScene>("res://scenes/entities/poker/poker_holder.tscn");
}