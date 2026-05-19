using GFramework.Core.Abstractions.controller;
using GFramework.Core.extensions;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using Godot;
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

    private void Add(IPokerView poker)
    {
        var holder = new Panel
        {
            Modulate = new Color("ffffff00"),
            SizeFlagsHorizontal = SizeFlags.ExpandFill
        };

        HolderContainer.AddChild(holder);
        PokerContainer.AddChild(poker as Node);
        if (poker is Control ctrl)
            ctrl.SetAnchorsPreset(Control.LayoutPreset.TopLeft);

        Mapping[holder] = poker;
        this.GetSystem<DeckSortSystem>().InitMapping(poker, holder);
    }

    private void Remove(IPokerView poker)
    {
        Panel holder = null!;
        foreach (var kvp in Mapping)
        {
            if (kvp.Value == poker)
            {
                holder = kvp.Key;
                break;
            }
        }
        if (holder == null!) return;

        Mapping.Remove(holder);
        this.GetSystem<DeckSortSystem>().RemoveBundle(poker.Id);
        holder.QueueFree();

        ReLayout();
    }

    private void ReorderChildrenToMatchModel()
    {
        var model = this.GetModel<DeckModel>();
        var sortedIds = model.Pokers;

        var pokerToHolder = new Dictionary<Guid, Panel>();
        foreach (var (holder, poker) in Mapping)
            pokerToHolder[poker.Id] = holder;

        for (int i = 0; i < sortedIds.Count; i++)
        {
            if (pokerToHolder.TryGetValue(sortedIds[i], out var holder))
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

        Panel currentHolder = null!;
        foreach (var kvp in Mapping)
        {
            if (kvp.Value == poker)
            {
                currentHolder = kvp.Key;
                break;
            }
        }
        if (currentHolder == null!) return;

        int currentIndex = currentHolder.GetIndex();
        if (currentIndex < 0 || currentIndex == targetIndex) return;

        HolderContainer.MoveChild(currentHolder, targetIndex);
        this.GetModel<DeckModel>().CurrentSortMode = DeckSortMode.Manual;

        ReLayout();
    }

    private void SynchronizePokerOrder()
    {
        var holders = HolderContainer.GetChildren();
        var targetPokerList = new List<IPokerView>();
        foreach (var child in holders)
        {
            if (child is Panel panel && Mapping.TryGetValue(panel, out var poker))
                targetPokerList.Add(poker);
        }

        for (int i = 0; i < targetPokerList.Count; i++)
        {
            if (targetPokerList[i] is Node node && node.GetParent() == PokerContainer)
                PokerContainer.MoveChild(node, i);
        }
    }
    
    private void ReLayout()
    {
        SynchronizePokerOrder();

        int count = Mathf.Min(PokerContainer.GetChildCount(), HolderContainer.GetChildCount());
        for (int i = 0; i < count; i++)
        {
            if (HolderContainer.GetChild(i) is not Control holder) continue;
            if (PokerContainer.GetChild(i) is not IPokerView poker) continue;
            if (!Mapping.ContainsKey((holder as Panel)!)) continue;

            Vector2 targetPos = holder.GlobalPosition + holder.Size / 2f;
            poker.ResetPosition = targetPos - poker.Size / 2f;
            poker.ResetRotation = 0f;
            this.SendCommand(new PokerUpdateViewPositionCommand{ PokerId = poker.Id, TargetPosition = poker.ResetPosition});
        }

        this.SendEvent(new DeckSortFinishedEvent());
    }
}