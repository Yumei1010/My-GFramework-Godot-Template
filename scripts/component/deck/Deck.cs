using GFramework.Core.Abstractions.controller;
using GFramework.Core.extensions;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using Godot;
using TimeToTwentyfour.scripts.cqrs.deck.@event;
using TimeToTwentyfour.scripts.entities.poker;
using TimeToTwentyfour.scripts.model.poker;

namespace TimeToTwentyfour.scripts.component.deck;

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

    /// <summary>获取牌桌上当前所有牌的列表（每次访问遍历子节点构建新列表）。</summary>
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

    /// <summary>向牌桌添加一张牌，为其创建透明的 holder 占位面板并维护映射关系。</summary>
    public void Add(IPokerView poker)
    {
        var holder = new Panel();
        holder.Modulate = new Color("ffffff00");
        holder.SizeFlagsHorizontal = SizeFlags.ExpandFill;

        HolderContainer.AddChild(holder);
        PokerContainer.AddChild(poker as Node);

        Mapping[holder] = poker;
    }

    /// <summary>从牌桌移除一张牌，释放其 holder 并触发布局重排。</summary>
    public void Remove(IPokerView poker)
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
        if (a is not IPokerData pa || b is not IPokerData pb) return 0;
        return DeckComparer.CompareBySuit(pa, pb);
    }

    private static int RankComparer(Node a, Node b)
    {
        if (a is not IPokerData pa || b is not IPokerData pb) return 0;
        return DeckComparer.CompareByRank(pa, pb);
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
    
    /// <summary>将拖拽结束的牌插入到离其横坐标最近的卡槽位置。</summary>
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
        CurrentSortMode = SortMode.Manual;

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
            poker.MoveTo(poker.ResetPosition);
        }

        this.SendEvent(new DeckSortFinishedEvent());
    }
}