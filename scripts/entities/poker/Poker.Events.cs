using GFramework.Core.extensions;
using GFramework.Godot.extensions;
using TimeToTwentyfour.scripts.cqrs.poker.command;
using TimeToTwentyfour.scripts.cqrs.poker.@event;
using TimeToTwentyfour.scripts.cqrs.selector.@event;
using TimeToTwentyfour.scripts.enums.poker;

namespace TimeToTwentyfour.scripts.entities.poker;

public partial class Poker
{
    private void RegisterEvent()
    {
        this.RegisterEvent<SelectorEnableChangedEvent>(e =>
        {
            OnSelectorEnableChangedEvent(e.Enable);
        }).UnRegisterWhenNodeExitTree(this);

        this.RegisterEvent<SelectorSelectChangedEvent>(e =>
        {
            OnSelectorSelectChangedEvent(e.IsSelected, e.PokerId);
        }).UnRegisterWhenNodeExitTree(this);

        this.RegisterEvent<PokerCardChangedEvent>(e =>
        {
            OnCardChangedEvent(e.Id, e.SuitType, e.NumValue, e.NumType);
        }).UnRegisterWhenNodeExitTree(this);
    }

    private void OnSelectorEnableChangedEvent(bool enable)
    {
        ChangeTo(enable ? StateType.UnSelect : StateType.Idle);
    }

    private void OnSelectorSelectChangedEvent(bool isSelected, Guid pokerId)
    {
        if (pokerId != Id) return;
        ChangeTo(isSelected ? StateType.OnSelect : StateType.UnSelect);
    }

    private void OnCardChangedEvent(Guid id, SuitType suitType, string numValue, NumType numType)
    {
        if (id != Id) return;
        SuitType = suitType;
        NumValue = numValue;
        NumType = numType;
        this.SendCommand(new PokerUpdateThemeCommand { PokerId = Id, SuitType = suitType, NumValue = numValue });
    }
}