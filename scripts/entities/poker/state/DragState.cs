using GFramework.Core.extensions;
using TimeToTwentyfour.scripts.enums.poker;
using Godot;
using TimeToTwentyfour.scripts.cqrs.poker.@event;

namespace TimeToTwentyfour.scripts.entities.poker.state;

/// <summary>
///     扑克拖拽状态，处理鼠标拖拽时的位置跟随与释放逻辑。
/// </summary>
public partial class DragState : PokerState
{
    public override void GuiInput(InputEvent inputEvent)
    {

    }

    public override void Process(double delta)
    {
        Poker.GlobalPosition = Poker.GetGlobalMousePosition() - Poker.Size / 2;
    }

    public override void Enter()
    {
        // 发送扑克开始拖拽事件
        this.SendEvent(new PokerDragStartedEvent
        {
            Poker = Poker
        });
        
        // 隐藏并锁定鼠标在窗口范围内
        Input.SetMouseMode(Input.MouseModeEnum.ConfinedHidden);
        
        Poker.TopLevel = true;
    }

    public override void Exit()
    {
        // 发送扑克结束拖拽事件
        this.SendEvent(new PokerDragFinishedEvent()
        {
            Poker = Poker
        });
        
        // 显示鼠标
        Input.SetMouseMode(Input.MouseModeEnum.Visible);
        
        Poker.TopLevel = false;
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