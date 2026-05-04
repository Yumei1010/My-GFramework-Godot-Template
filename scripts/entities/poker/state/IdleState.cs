using Godot;
using TimeToTwentyfour.scripts.enums.poker;

namespace TimeToTwentyfour.scripts.entities.poker.state;

/// <summary>
///     扑克空闲状态，等待玩家交互输入以切换到选中或拖拽状态。
/// </summary>
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