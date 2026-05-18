using GFramework.Core.extensions;
using GFramework.Godot.extensions;
using TimeToTwentyfour.scripts.cqrs.deck.@event;
using TimeToTwentyfour.scripts.cqrs.poker.@event;
using TimeToTwentyfour.scripts.enums.deck;
using TimeToTwentyfour.scripts.model.deck;
using TimeToTwentyfour.scripts.system.poker;

namespace TimeToTwentyfour.scripts.entities.deck;

public partial class Deck
{
    private void RegisterEvent()
    {
        this.RegisterEvent<PokerDragFinishedEvent>(e =>
        {
            OnPokerDragFinishedEvent(e.PokerId);
        }).UnRegisterWhenNodeExitTree(this);

        this.RegisterEvent<DeckPokerAddedEvent>(e =>
        {
            OnPokerAddedEvent(e.PokerId);
        }).UnRegisterWhenNodeExitTree(this);

        this.RegisterEvent<DeckPokerRemovedEvent>(e =>
        {
            OnPokerRemovedEvent(e.PokerId);
        }).UnRegisterWhenNodeExitTree(this);

        this.RegisterEvent<DeckSortStartedEvent>(_ =>
        {
            OnSortStartedEvent();
        }).UnRegisterWhenNodeExitTree(this);

        this.RegisterEvent<DeckSortFinishedEvent>(_ =>
        {
            OnSortFinishedEvent();
        }).UnRegisterWhenNodeExitTree(this);
    }

    private void OnPokerDragFinishedEvent(Guid pokerId)
    {
        var poker = this.GetSystem<PokerSystem>().Find(pokerId);
        if (poker == null) return;
        float pokerCenterX = poker.GlobalPosition.X - poker.Size.X / 2f;
        InsertPokerAtNearestSlot(poker, pokerCenterX);
    }

    private void OnPokerAddedEvent(Guid pokerId)
    {
        var poker = this.GetSystem<PokerSystem>().Find(pokerId);
        if (poker == null) return;
        Add(poker);
        ReLayout();
    }

    private void OnPokerRemovedEvent(Guid pokerId)
    {
        var poker = this.GetSystem<PokerSystem>().Find(pokerId);
        if (poker == null) return;
        Remove(poker);
    }

    private void OnSortStartedEvent()
    {
        var model = this.GetModel<DeckModel>();
        switch (model.CurrentSortMode)
        {
            case DeckSortMode.Suit:
                SortBySuit();
                break;
            case DeckSortMode.Value:
                SortByValue();
                break;
        }
    }

    private void OnSortFinishedEvent()
    {
        var model = this.GetModel<DeckModel>();
        switch (model.CurrentSortMode)
        {
            case DeckSortMode.Suit:
                SortBySuit();
                break;
            case DeckSortMode.Value:
                SortByValue();
                break;
        }
    }
}
