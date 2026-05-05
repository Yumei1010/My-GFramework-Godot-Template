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
            OnSelectorSelectChangedEvent(e.IsSelected, e.Poker);
        }).UnRegisterWhenNodeExitTree(this);

        this.RegisterEvent<SelectorEnableChangedEvent>(e =>
        {
            Enable = e.Enable;
        }).UnRegisterWhenNodeExitTree(this);
    }

    /// <summary>响应选择变更事件：选中时加入队列，取消选中时移除；选择器未启用时忽略。</summary>
    private void OnSelectorSelectChangedEvent(bool isSelected, IPoker poker)
    {
        if (!Enable) return;
        if (isSelected)
        {
            _selection.Add(poker);
        }
        else
        {
            _selection.Remove(poker);
        }
    }
}
