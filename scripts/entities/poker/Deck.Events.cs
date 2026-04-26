using GFramework.Core.extensions;
using GFramework.Godot.extensions;
using Godot;
using TimeToTwentyfour.scripts.cqrs.poker.@event;

namespace TimeToTwentyfour.scripts.entities.poker;

public partial class Deck
{
    private void RegisterEvent()
    {
        // 注册对扑克结束拖拽事件的监听
        this.RegisterEvent<PokerDragFinishedEvent>(e =>
        {
            OnPokerDragFinishedEvent(e.Poker);
        }).UnRegisterWhenNodeExitTree(this);
        
        // 注册对卡套被抽取事件的监听
        this.RegisterEvent<PokerHolderExtractedEvent>(e =>
        {
            OnPokerHolderExtractEvent(e.Poker);
        }).UnRegisterWhenNodeExitTree(this);
    }
    
    private void OnPokerDragFinishedEvent(IPoker poker)
    {
        HolderContainer.MoveChild(FindHolderFromPoker(poker) as Node, HolderContainer.GetChildCount());
    }

    private void OnPokerHolderExtractEvent(IPoker poker)
    {
        // 从卡册中移除被抽出扑克的卡套
        Album.Remove(FindHolderFromPoker(poker));
    }
}