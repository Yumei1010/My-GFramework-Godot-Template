using Godot;
using TimeToTwentyfour.scripts.enums.poker;

namespace TimeToTwentyfour.scripts.entities.poker.state;

public partial class DisplayState : PokerState
{
    public override void GuiInput(InputEvent inputEvent)
    {
        if (Poker.GetFake3D())
        {
            float rotX = Mathf.RadToDeg(Mathf.LerpAngle(Mathf.DegToRad(-5f), Mathf.DegToRad(5f), Mathf.Clamp(Poker.GetLocalMousePosition().X / Poker.GetSize().X, 0f, 1f)));
            float rotY = Mathf.RadToDeg(Mathf.LerpAngle(Mathf.DegToRad(5f), Mathf.DegToRad(-5f), Mathf.Clamp(Poker.GetLocalMousePosition().Y / Poker.GetSize().Y, 0f, 1f)));
            Poker.SetXRotAndYRot(rotX, rotY);
        }
    }

    public override void MouseDown()
    {
        ChangeTo(StateType.Idle);
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

    public override void Process(double delta)
    {
        
    }

    public override void Enter()
    {
        
    }

    public override void Exit()
    {
        
    }
}