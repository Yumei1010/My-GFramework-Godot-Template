using GFrameworkGodotTemplate.scripts.enums.poker;
using Godot;

namespace GFrameworkGodotTemplate.scripts.poker.state;

public partial class OnSelectState : PokerState
{
    public override void Process(double delta)
    {
        
    }

    public override void Enter()
    {
        Vector2 pos = Poker.GetGlobalPosition();
        pos.Y -= Poker.GetSize().Y / 2;   
        Poker.SetPos(pos);
    }

    public override void Exit()
    {
        Vector2 pos = Poker.GetGlobalPosition();
        pos.Y += Poker.GetSize().Y / 2;   
        Poker.SetPos(pos);
    }

    public override void MouseDown()
    {
        RequestStateChange(StateType.UnSelect);
    }

    public override void MouseUp()
    {
        
    }

    public override void MouseEnter()
    {
        
    }
    
    public override void MouseExit()
    {
        
    }
}