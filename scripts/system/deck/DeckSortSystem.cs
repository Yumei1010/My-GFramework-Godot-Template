using GFramework.Core.Abstractions.enums;
using GFramework.Core.Abstractions.system;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using Godot;
using TimeToTwentyfour.scripts.entities.poker;

namespace TimeToTwentyfour.scripts.system.deck;

[Log]
[ContextAware]
public partial class DeckSortSystem : ISystem
{
    private struct MappingBundle
    {
        public IPokerView Poker { get; set; }
        public Panel Holder { get; set; }
    }

    private struct Comparer
    {
        /// <summary>
        ///     按花色降序排列（♣ > ♠ > ♦ > ♥），同花色按 <see cref="PokerNumType"/> 升序。
        /// </summary>
        public static int CompareBySuit(IPokerData a, IPokerData b)
        {
            int suitCmp = b.PokerSuitType.CompareTo(a.PokerSuitType);
            return suitCmp != 0 ? suitCmp : a.PokerNumType.CompareTo(b.PokerNumType);
        }

        /// <summary>
        ///     按 <see cref="PokerNumType"/> 升序排列，同点数按花色降序。
        /// </summary>
        public static int CompareByValue(IPokerData a, IPokerData b)
        {
            int rankCmp = a.PokerNumType.CompareTo(b.PokerNumType);
            return rankCmp != 0 ? rankCmp : b.PokerSuitType.CompareTo(a.PokerSuitType);
        }
    }

    private Dictionary<Guid, MappingBundle> Bundles = [];

    public void OnArchitecturePhase(ArchitecturePhase phase)
    {
        _log.Debug("System initialized: DeckSystem");
    }

    public void Init()
    {
        
    }

    public void Destroy()
    {
        
    }

    public void InitMappings(IPokerView poker, Panel holder)
    {
        Bundles[poker.Id] = new MappingBundle
        {
            Poker = poker,
            Holder = holder
        };
    }

    public void RemoveBundle(Guid id)
    {
        Bundles.Remove(id);
    }

    public void Sort()
    {
        
    }
}