using Godot;
using TimeToTwentyfour.scripts.enums.poker;

namespace TimeToTwentyfour.scripts.entities.poker.state;

public partial class IdleState : PokerState
{
    public override void GuiInput(InputEvent inputEvent)
    {
        if (inputEvent.IsActionPressed("MouseLeft"))
        {
            ChangeTo(StateType.Drag);
        }
        else if (inputEvent.IsActionPressed("MouseRight"))
        {
            ChangeTo(StateType.Display);
        }
    }
    
    public override void Process(double delta)
    {

    }

    public override void Enter()
    {
        
    }

    public override void Exit()
    {
        
    }

    public override void MouseDown()
    {
        
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