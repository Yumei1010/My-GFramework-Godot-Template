using TimeToTwentyfour.scripts.entities.poker;
using TimeToTwentyfour.scripts.utility;
using Godot;

namespace TimeToTwentyfour.scripts.component;

public partial class HandHolder : Control
{
    private Control PokerHolderContainer => GetNode<Control>("%PokerHolderContainer");

    private IList<IPoker> Hands { get; set; } = new List<IPoker>();

    public override void _Ready()
    {
        RegisterEvent();
        UpdateHands();
    }

    private void RegisterEvent()
    {
        
    }

    private void UpdateHands()
    {
        Hands.Clear();
        
        foreach (var node in PokerHolderContainer.GetChildren())
        {
            var poker = (IPoker)node;
            Hands.Add(poker);
        }
        
        Sort();
    }
    
    private void Sort()
    {
        foreach (var poker in Hands)
        {
            Vector2 pos = HandHolderHelper.GetPosition(Hands.Count, Hands.IndexOf(poker));
            float angle = HandHolderHelper.GetAngle(Hands.Count, Hands.IndexOf(poker));
            
            poker.SetDefaultPosition(pos);
            poker.SetDefaultRotation(angle);
        }
    }
}