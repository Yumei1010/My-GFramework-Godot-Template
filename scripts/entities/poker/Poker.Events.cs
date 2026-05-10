using GFramework.Core.extensions;
using GFramework.Godot.extensions;
using TimeToTwentyfour.scripts.cqrs.poker.@event;
using TimeToTwentyfour.scripts.cqrs.selector.@event;
using TimeToTwentyfour.scripts.enums.poker;

namespace TimeToTwentyfour.scripts.entities.poker;

public partial class Poker
{
    private void RegisterEvent()
    {
        // 注册对扑克选择器可用性变更事件的监听
        this.RegisterEvent<SelectorEnableChangedEvent>(e =>
        {
            OnSelectorEnableChangedEvent(e.Enable);
        }).UnRegisterWhenNodeExitTree(this);

        // 注册对选择变更事件的监听
        this.RegisterEvent<SelectorSelectChangedEvent>(e =>
        {
            OnSelectorSelectChangedEvent(e.IsSelected, e.Poker);
        }).UnRegisterWhenNodeExitTree(this);

        // 注册对预览运算结果变更事件的监听
        this.RegisterEvent<PokerReserveResultChangedEvent>(e =>
        {
            OnReserveResultChangedEvent(e.NumValue, e.IsHidden, e.Poker);
        }).UnRegisterWhenNodeExitTree(this);

        // 注册对卡牌模型数据变更事件的监听
        this.RegisterEvent<PokerCardChangedEvent>(e =>
        {
            OnCardChangedEvent(e.Id, e.SuitType, e.NumValue, e.NumType);
        }).UnRegisterWhenNodeExitTree(this);
    }

    private void OnSelectorEnableChangedEvent(bool enable)
    {
        ChangeTo(enable ? StateType.UnSelect : StateType.Idle);
    }

    private void OnSelectorSelectChangedEvent(bool isSelected, IPoker poker)
    {
        if (poker != this) return;
        ChangeTo(isSelected ? StateType.OnSelect : StateType.UnSelect);
    }

    private void OnReserveResultChangedEvent(String numValue,bool visible,IPoker poker)
    {
        // 如果不是触发事件的poker，返回
        if (poker != this) return;

        ReserveResultRect.Visible = visible;
        ReserveResultLabel.Text = numValue;
    }

    private void OnCardChangedEvent(Guid id ,SuitType suitType, String numValue, NumType numType)
    {
        if (id != Id) return;
        SuitType = suitType;
        NumValue = numValue;
        NumType = numType;
    }
}