using GFramework.Core.extensions;
using GFramework.Godot.extensions;
using TimeToTwentyfour.scripts.cqrs.selector.@event;
using TimeToTwentyfour.scripts.entities.poker;

namespace TimeToTwentyfour.scripts.component.selector;

public partial class Selector
{
    private void RegisterEvent()
    {
        this.RegisterEvent<SelectorSelectChangedEvent>(e =>
        {
            OnSelectorSelectChangedEvent(e.IsSelected,e.Poker);
        }).UnRegisterWhenNodeExitTree(this);

        this.RegisterEvent<SelectorEnableChangedEvent>(e =>
        {
            Enable = e.Enable;
        }).UnRegisterWhenNodeExitTree(this);
    }
    
    private void OnSelectorSelectChangedEvent(bool isSelected,IPoker poker)
    {
        if (!Enable) return;
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