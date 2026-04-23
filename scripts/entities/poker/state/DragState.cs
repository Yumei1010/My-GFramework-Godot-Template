using TimeToTwentyfour.scripts.enums.poker;
using Godot;

namespace TimeToTwentyfour.scripts.entities.poker.state;

public partial class DragState : PokerState
{
    public override void Process(double delta)
    {
        Poker.SetGlobalPosition(Poker.GetGlobalMousePosition() - Poker.GetSize() / 2);
    }

    public override void Enter()
    {
        // 隐藏并锁定鼠标在窗口范围内
        Input.SetMouseMode(Input.MouseModeEnum.ConfinedHidden);
    }

    public override void Exit()
    {
        // 显示鼠标
        Input.SetMouseMode(Input.MouseModeEnum.Visible);
    }

    public override void MouseDown()
    {
        
    }

    public override void MouseUp()
    {
        ChangeTo(StateType.Idle);
    }

    public override void MouseEnter()
    {
        
    }
    
    public override void MouseExit()
    {
        
    }
}