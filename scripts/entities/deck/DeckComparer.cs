using TimeToTwentyfour.scripts.enums.poker;
using TimeToTwentyfour.scripts.entities.poker;

namespace TimeToTwentyfour.scripts.entities.deck;

/// <summary>
///     Deck 排序比较器 —— 提供按花色、按点数的纯 <see cref="IPokerData"/> 比较逻辑，不依赖 Godot 节点。
/// </summary>
public static class DeckComparer
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
    public static int CompareByRank(IPokerData a, IPokerData b)
    {
        int rankCmp = a.PokerNumType.CompareTo(b.PokerNumType);
        return rankCmp != 0 ? rankCmp : b.PokerSuitType.CompareTo(a.PokerSuitType);
    }
}
