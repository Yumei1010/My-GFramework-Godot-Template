using GFramework.Core.Abstractions.enums;
using GFramework.Core.Abstractions.system;
using GFramework.Core.extensions;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using Godot;
using TimeToTwentyfour.scripts.cqrs.deck.@event;
using TimeToTwentyfour.scripts.entities.poker;
using TimeToTwentyfour.scripts.enums.deck;
using TimeToTwentyfour.scripts.model.deck;

namespace TimeToTwentyfour.scripts.system.deck;

[Log]
[ContextAware]
public partial class DeckSortSystem : ISystem
{
    public struct MappingBundle
    {
        public IPokerView Poker { get; set; }
        public Panel Holder { get; set; }
    }

    private struct Comparer
    {
        public static int BySuit(IPokerData a, IPokerData b)
        {
            int suitCmp = b.PokerSuitType.CompareTo(a.PokerSuitType);
            return suitCmp != 0 ? suitCmp : a.PokerNumType.CompareTo(b.PokerNumType);
        }

        public static int ByValue(IPokerData a, IPokerData b)
        {
            int rankCmp = a.PokerNumType.CompareTo(b.PokerNumType);
            return rankCmp != 0 ? rankCmp : b.PokerSuitType.CompareTo(a.PokerSuitType);
        }
    }

    private readonly Dictionary<Guid, MappingBundle> Bundles = [];

    public void OnArchitecturePhase(ArchitecturePhase phase)
    {
        _log.Debug("System initialized: DeckSystem");
    }

    public void Init() { }

    public void Destroy() { }

    public void InitMapping(IPokerView poker, Panel holder)
    {
        Bundles[poker.Id] = new MappingBundle { Poker = poker, Holder = holder };
    }

    public void RemoveBundle(Guid id)
    {
        Bundles.Remove(id);
    }

    public Panel FindHolder(Guid id)
    {
        return Bundles.TryGetValue(id, out var b) ? b.Holder : null!;
    }

    public IPokerView FindPoker(Guid id)
    {
        return Bundles.TryGetValue(id, out var b) ? b.Poker : null!;
    }

    public IEnumerable<MappingBundle> AllBundles => Bundles.Values;

    public void Sort()
    {
        var model = this.GetModel<DeckModel>();
        var pokers = model.Pokers;

        switch (model.CurrentSortMode)
        {
            case DeckSortMode.Suit:
                pokers.Sort((a, b) => Comparer.BySuit((IPokerData)Bundles[a].Poker, (IPokerData)Bundles[b].Poker));
                break;
            case DeckSortMode.Value:
                pokers.Sort((a, b) => Comparer.ByValue((IPokerData)Bundles[a].Poker, (IPokerData)Bundles[b].Poker));
                break;
        }

        this.SendEvent(new DeckSortStartedEvent());
    }
}