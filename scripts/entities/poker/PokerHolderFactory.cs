using Godot;

namespace TimeToTwentyfour.scripts.entities.poker;

public partial class PokerHolderFactory : Node, IPokerHolderFactory
{
    public IPokerHolder Product()
    {
        return  _pokerHolderScene.Instantiate<IPokerHolder>();
    }
}