using GFramework.Core.extensions;
using GFramework.Godot.extensions;
using TimeToTwentyfour.scripts.cqrs.poker.@event;
using TimeToTwentyfour.scripts.entities.poker;

namespace TimeToTwentyfour.scripts.entities.deck;

public partial class Deck
{
    private void RegisterEvent()
    {
        this.RegisterEvent<PokerDragFinishedEvent>(e =>
        {
            OnPokerDragFinishedEvent(e.Poker);
        }).UnRegisterWhenNodeExitTree(this);
    }

    private void OnPokerDragFinishedEvent(IPoker poker)
    {
        float pokerCenterX = poker.GlobalPosition.X - poker.Size.X / 2f;
        InsertPokerAtNearestSlot(poker, pokerCenterX);
    }
}