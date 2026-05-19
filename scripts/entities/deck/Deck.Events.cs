using GFramework.Core.extensions;
using GFramework.Godot.extensions;
using Godot;
using TimeToTwentyfour.scripts.cqrs.deck.@event;
using TimeToTwentyfour.scripts.cqrs.poker.@event;
using TimeToTwentyfour.scripts.system.deck;
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

        var holder = new Panel
        {
            Modulate = new Color("ffffff00"),
            SizeFlagsHorizontal = SizeFlags.ExpandFill
        };

        HolderContainer.AddChild(holder);
        PokerContainer.AddChild(poker as Node);

        this.GetSystem<DeckSortSystem>().InitMapping(poker, holder);
        ReLayout();
    }

    private void OnPokerRemovedEvent(Guid pokerId)
    {
        var poker = this.GetSystem<PokerSystem>().Find(pokerId);
        if (poker == null) return;
        
        var holder = this.GetSystem<DeckSortSystem>().FindHolder(poker.Id);
        if (holder == null) return;

        this.GetSystem<DeckSortSystem>().RemoveBundle(poker.Id);
        holder.QueueFree();
        ReLayout();
    }

    private void OnSortStartedEvent()
    {
        ReorderChildrenToMatchModel();
    }
}
