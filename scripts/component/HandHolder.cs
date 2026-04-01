using GFrameworkGodotTemplate.scripts.enums.poker;
using GFrameworkGodotTemplate.scripts.poker;
using GFrameworkGodotTemplate.scripts.utility;
using Godot;
using Godot.Collections;

namespace GFrameworkGodotTemplate.scripts.component;

public partial class HandHolder : Control
{
    // private Control PokerHolderContainer => GetNode<Control>("%PokerHolderContainer");
    //
    // public IList<Poker> Hands = new Array<Poker>();
    //
    // public override void _Ready()
    // {
    //     Sort();
    // }
    //
    // private void Sort()
    // {
    //     foreach (var node in PokerHolderContainer.GetChildren())
    //     {
    //         var poker = (Poker)node;
    //         Hands.Add(poker);
    //     }
    //
    //     for (int i = 0; i < Hands.Count; i++)
    //     {
    //         Hands[i].HandPosition = HandHolderHelper.GetPosition(Hands.Count,i);
    //         Hands[i].HandRotation = HandHolderHelper.GetAngle(Hands.Count,i);
    //         
    //         Hands[i].GlobalPosition = HandHolderHelper.GetPosition(Hands.Count,i);
    //         Hands[i].Rotation = Mathf.DegToRad(HandHolderHelper.GetAngle(Hands.Count,i));
    //     }
    // }
}