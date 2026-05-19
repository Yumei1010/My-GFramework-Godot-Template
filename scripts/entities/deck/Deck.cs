using GFramework.Core.Abstractions.controller;
using GFramework.Core.extensions;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using Godot;
using TimeToTwentyfour.scripts.cqrs.deck.command;
using TimeToTwentyfour.scripts.cqrs.deck.@event;
using TimeToTwentyfour.scripts.cqrs.poker.command;
using TimeToTwentyfour.scripts.entities.poker;
using TimeToTwentyfour.scripts.enums.deck;
using TimeToTwentyfour.scripts.model.deck;
using TimeToTwentyfour.scripts.system.deck;

namespace TimeToTwentyfour.scripts.entities.deck;

[Log]
[ContextAware]
public partial class Deck : Control, IDeck, IController
{
    public override void _Ready()
    {
        _ = ReadyAsync();
        ConnectSignal();
        RegisterEvent();
    }

    public IList<IPokerView> Pokers
    {
        get
        {
            var list = new List<IPokerView>();
            foreach (var child in PokerContainer.GetChildren())
            {
                if (child is IPokerView poker)
                    list.Add(poker);
            }
            return list;
        }
    }

    private void ReorderChildrenToMatchModel()
    {
        var sortedIds = this.GetModel<DeckModel>().Pokers;

        for (int i = 0; i < sortedIds.Count; i++)
        {
            var holder = this.GetSystem<DeckSortSystem>().FindHolder(sortedIds[i]);
            if (holder != null)
                HolderContainer.MoveChild(holder, i);
        }

        SynchronizePokerOrder();
        ReLayout();
    }

    private void InsertPokerAtNearestSlot(IPokerView poker, float globalX)
    {
        int count = HolderContainer.GetChildCount();
        if (count <= 1) return;

        int targetIndex = 0;
        float minDistance = float.MaxValue;
        for (int i = 0; i < count; i++)
        {
            var h = HolderContainer.GetChild(i) as Control;
            if (h == null) continue;
            float centerX = h.GlobalPosition.X + h.Size.X / 2f;
            float dist = Mathf.Abs(globalX - centerX);
            if (dist < minDistance)
            {
                minDistance = dist;
                targetIndex = i;
            }
        }

        var currentHolder = this.GetSystem<DeckSortSystem>().FindHolder(poker.Id);
        if (currentHolder == null) return;

        int currentIndex = currentHolder.GetIndex();
        if (currentIndex < 0 || currentIndex == targetIndex) return;

        HolderContainer.MoveChild(currentHolder, targetIndex);
        this.SendCommand(new DeckChangeSortModeCommand { TargetSortMode = DeckSortMode.Manual});

        ReLayout();
    }

    private void SynchronizePokerOrder()
    {
        var bundles = this.GetSystem<DeckSortSystem>().AllBundles
            .OrderBy(b => b.Holder.GetIndex())
            .ToList();

        for (int i = 0; i < bundles.Count; i++)
        {
            if (bundles[i].Poker is Node node && node.GetParent() == PokerContainer)
                PokerContainer.MoveChild(node, i);
        }
    }

    private void ReLayout()
    {
        SynchronizePokerOrder();

        foreach (var bundle in this.GetSystem<DeckSortSystem>().AllBundles)
        {
            var holder = bundle.Holder;
            var poker = bundle.Poker;
            Vector2 targetPos = holder.GlobalPosition + holder.Size / 2f;
            poker.ResetPosition = targetPos - poker.Size / 2f;
            poker.ResetRotation = 0f;
            this.SendCommand(new PokerUpdateViewPositionCommand { PokerId = poker.Id, TargetPosition = poker.ResetPosition, Animated = true });
        }

        this.SendEvent(new DeckSortFinishedEvent());
    }
}
