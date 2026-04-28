using Godot;
using TimeToTwentyfour.scripts.enums.poker;

namespace TimeToTwentyfour.scripts.entities.poker.state;

public partial class IdleState : PokerState
{
    public override void GuiInput(InputEvent inputEvent)
    {

    }
    
    public override void Process(double delta)
    {

    }

    public override void Enter()
    {
        Poker.Reset("Position");
    }

    public override void Exit()
    {
        
    }

    public override void MouseDown()
    {
        ChangeTo(StateType.Drag);
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