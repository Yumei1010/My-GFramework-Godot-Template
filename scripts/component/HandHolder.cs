using GFrameworkGodotTemplate.scripts.enums.calculate;
using GFrameworkGodotTemplate.scripts.enums.poker;
using GFrameworkGodotTemplate.scripts.poker;
using GFrameworkGodotTemplate.scripts.utility;
using Godot;

namespace GFrameworkGodotTemplate.scripts.component;

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
        
        Hands[0].SetNumValue("12");
        Hands[0].SetNumType(NumType.Integer);
        
        Hands[1].SetNumValue("24");
        Hands[1].SetNumType(NumType.Integer);
        
        GD.Print("sum = ",CalculateHelper.Calculate(Hands[0],Hands[1],OperateType.Power));
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