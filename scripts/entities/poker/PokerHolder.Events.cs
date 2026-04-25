using GFramework.Core.extensions;
using GFramework.Godot.extensions;
using TimeToTwentyfour.scripts.cqrs.poker.@event;

namespace TimeToTwentyfour.scripts.entities.poker;

public partial class PokerHolder
{
    private void RegisterEvent()
    {
        // 注册对扑克结束拖拽事件的监听
        this.RegisterEvent<PokerDragFinishedEvent>(e =>
        {
            OnPokerDragFinishedEvent(e.Poker);
        }).UnRegisterWhenNodeExitTree(this);
        
        // 注册对牌桌结束排序事件的监听
        this.RegisterEvent<DeckSortFinishedEvent>(_ =>
        {
            OnDeckSortFinishedEvent();
        }).UnRegisterWhenNodeExitTree(this);
    }
    
    private void OnPokerDragFinishedEvent(IPoker poker)
    {
        // 如果不是触发事件的poker，返回
        if (Poker != poker) return;
        
        Poker.MoveTo(GlobalPosition);
    }

    private void OnDeckSortFinishedEvent()
    {
        Poker.SetDefaultPosition(GlobalPosition);
        Poker.MoveTo(GlobalPosition);
    }
}