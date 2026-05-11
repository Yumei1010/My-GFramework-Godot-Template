using GFramework.Core.extensions;
using GFramework.Godot.extensions;
using TimeToTwentyfour.scripts.cqrs.poker.@event;
using TimeToTwentyfour.scripts.entities.poker;

namespace TimeToTwentyfour.scripts.component.deck;

public partial class Deck
{
    private void RegisterEvent()
    {
        ContextAwareExtensions.RegisterEvent<PokerDragFinishedEvent>(this, e =>
        {
            OnPokerDragFinishedEvent(e.PokerId);
        }).UnRegisterWhenNodeExitTree(this);
    }

    private void OnPokerDragFinishedEvent(Guid pokerId)
    {
        var poker = PokerRegistry.Find(pokerId);
        if (poker == null) return;
        float pokerCenterX = poker.GlobalPosition.X - poker.Size.X / 2f;
        InsertPokerAtNearestSlot(poker, pokerCenterX);
    }
}