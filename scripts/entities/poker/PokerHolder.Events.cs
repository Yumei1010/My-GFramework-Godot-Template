using GFramework.Core.extensions;
using GFramework.Godot.extensions;
using TimeToTwentyfour.scripts.cqrs.poker.@event;

namespace TimeToTwentyfour.scripts.entities.poker;

public partial class PokerHolder
{
    private void RegisterEvent()
    {
        // 注册对扑克开始拖拽事件的监听
        this.RegisterEvent<PokerDragStartedEvent>(e =>
        {
            OnPokerDragStartedEvent(e.Poker);
        }).UnRegisterWhenNodeExitTree(this);
        
        // 注册对扑克结束拖拽事件的监听
        this.RegisterEvent<PokerDragFinishedEvent>(e =>
        {
            OnPokerDragFinishedEvent(e.Poker);
        }).UnRegisterWhenNodeExitTree(this);
    }

    private void OnPokerDragStartedEvent(IPoker poker)
    {
        // 如果不是触发事件的poker，返回
        if (Poker != poker) return;
        
        Poker.ResetRot();
    }
    
    private void OnPokerDragFinishedEvent(IPoker poker)
    {
        // 如果不是触发事件的poker，返回
        if (Poker != poker) return;
        
        Poker.MoveTo(GlobalPosition);
    }
}