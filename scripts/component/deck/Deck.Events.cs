using GFramework.Core.extensions;
using GFramework.Godot.extensions;
using TimeToTwentyfour.global;
using TimeToTwentyfour.scripts.cqrs.poker.@event;

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
        var poker = GetNode<PokerManager>("/root/PokerManager").Find(pokerId);
        if (poker == null) return;
        float pokerCenterX = poker.GlobalPosition.X - poker.Size.X / 2f;
        InsertPokerAtNearestSlot(poker, pokerCenterX);
    }
}