using GFrameworkGodotTemplate.scripts.poker;
using Godot;

namespace GFrameworkGodotTemplate.scripts.component;

public partial class HandHolder : Control
{
    private Control PokerHolderContainer => GetNode<Control>("%PokerHolderContainer");

    public IList<IPoker> Hands = null!;
}