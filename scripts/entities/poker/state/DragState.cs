using GFramework.Core.extensions;
using TimeToTwentyfour.scripts.enums.poker;
using Godot;
using TimeToTwentyfour.scripts.cqrs.poker.@event;

namespace TimeToTwentyfour.scripts.entities.poker.state;

public partial class DragState : PokerState
{
    public override void Process(double delta)
    {
        Poker.SetGlobalPosition(Poker.GetGlobalMousePosition() - Poker.GetSize() / 2);
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
        
        // 偏转角度归零
        Poker.SetRotation(0f);
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
        
        // 复位偏转角度
        Poker.ResetRot();
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