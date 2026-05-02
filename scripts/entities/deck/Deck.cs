using GFramework.Core.Abstractions.controller;
using GFramework.Core.extensions;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using Godot;
using TimeToTwentyfour.scripts.cqrs.deck.@event;
using TimeToTwentyfour.scripts.entities.poker;

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

    public IList<IPoker> Pokers
    {
        get
        {
            var list = new List<IPoker>();
            foreach (var child in PokerContainer.GetChildren())
            {
                if (child is IPoker poker)
                    list.Add(poker);
            }
            return list;
        }
    }

    public void Add(IPoker poker)
    {
        var holder = new Panel();
        holder.Modulate = new Color("ffffff00");
        holder.SizeFlagsHorizontal = SizeFlags.ExpandFill;

        HolderContainer.AddChild(holder);
        PokerContainer.AddChild(poker as Node);

        Mapping[holder] = poker;
    }

    public void Remove(IPoker poker)
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
        holder.QueueFree();

        ReLayout();
    }

    public void SortBySuit()
    {
        CurrentSortMode = SortMode.BySuit;
        ReorderChildren(SuitComparer);
    }

    public void SortByRank()
    {
        CurrentSortMode = SortMode.ByRank;
        ReorderChildren(RankComparer);
    }

    private static int SuitComparer(Node a, Node b)
    {
        if (a is not IPoker pa || b is not IPoker pb) return 0;
        int suitCmp = pb.SuitType.CompareTo(pa.SuitType);
        return suitCmp != 0 ? suitCmp : pa.NumType.CompareTo(pb.NumType);
    }

    private static int RankComparer(Node a, Node b)
    {
        if (a is not IPoker pa || b is not IPoker pb) return 0;
        int rankCmp = pa.NumType.CompareTo(pb.NumType);
        return rankCmp != 0 ? rankCmp : pb.SuitType.CompareTo(pa.SuitType);
    }

    private void ReorderChildren(Comparison<Node> comparison)
    {
        int count = PokerContainer.GetChildCount();
        var pairs = new List<(Node holder, Node poker)>();
        for (int i = 0; i < count; i++)
        {
            pairs.Add((HolderContainer.GetChild(i), PokerContainer.GetChild(i)));
        }

        pairs.Sort((a, b) => comparison(a.poker, b.poker));

        for (int i = 0; i < count; i++)
        {
            var (holder, poker) = pairs[i];
            HolderContainer.MoveChild(holder, i);
            PokerContainer.MoveChild(poker, i);
        }

        ReLayout();
    }
    
    private void InsertPokerAtNearestSlot(IPoker poker, float globalX)
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
        CurrentSortMode = SortMode.Manual;

        ReLayout();
    }

    private void SynchronizePokerOrder()
    {
        var holders = HolderContainer.GetChildren();
        var targetPokerList = new List<IPoker>();
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
            if (PokerContainer.GetChild(i) is not IPoker poker) continue;
            if (!Mapping.ContainsKey((holder as Panel)!)) continue;

            Vector2 targetPos = holder.GlobalPosition + holder.Size / 2f;
            poker.ResetPosition = targetPos - poker.Size / 2f;
            poker.ResetRotation = 0f;
            poker.MoveTo(poker.ResetPosition);
        }

        this.SendEvent(new DeckSortFinishedEvent());
    }
}