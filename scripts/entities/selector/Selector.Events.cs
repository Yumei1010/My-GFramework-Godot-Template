using GFramework.Core.extensions;
using GFramework.Godot.extensions;
using TimeToTwentyfour.scripts.cqrs.selector.@event;
using TimeToTwentyfour.scripts.entities.poker;

namespace TimeToTwentyfour.scripts.entities.selector;

public partial class Selector
{
    private void RegisterEvent()
    {
        // 注册对选择变更事件的监听
        this.RegisterEvent<SelectorSelectChangedEvent>(e =>
        {
            OnSelectorSelectChangedEvent(e.IsSelected,e.Poker);
        }).UnRegisterWhenNodeExitTree(this);
    }
    
    private void OnSelectorSelectChangedEvent(bool isSelected,IPoker poker)
    {
        if (isSelected)
        {
            Add(poker);
        }
        else
        {
            Remove(poker);
        }
    }
}