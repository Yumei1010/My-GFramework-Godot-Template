using GFramework.Core.extensions;
using GFramework.Godot.extensions;
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
    }

    private void OnSelectorEnableChangedEvent(bool enable)
    {
        ChangeTo(enable ? PokerStateType.UnSelect : PokerStateType.Idle);
    }

    private void OnSelectorSelectChangedEvent(bool isSelected, Guid pokerId)
    {
        if (pokerId != Id) return;
        ChangeTo(isSelected ? PokerStateType.OnSelect : PokerStateType.UnSelect);
    }
}