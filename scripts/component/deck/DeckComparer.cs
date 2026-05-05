using TimeToTwentyfour.scripts.entities.poker;
using TimeToTwentyfour.scripts.enums.poker;

namespace TimeToTwentyfour.scripts.component.deck;

/// <summary>
///     Deck 排序比较器 —— 提供按花色、按点数的纯 <see cref="IPoker"/> 比较逻辑，不依赖 Godot 节点。
/// </summary>
public static class DeckComparer
{
    /// <summary>
    ///     按花色降序排列（♣ > ♠ > ♦ > ♥），同花色按 <see cref="NumType"/> 升序。
    /// </summary>
    public static int CompareBySuit(IPoker a, IPoker b)
    {
        int suitCmp = b.SuitType.CompareTo(a.SuitType);
        return suitCmp != 0 ? suitCmp : a.NumType.CompareTo(b.NumType);
    }

    /// <summary>
    ///     按 <see cref="NumType"/> 升序排列，同点数按花色降序。
    /// </summary>
    public static int CompareByRank(IPoker a, IPoker b)
    {
        int rankCmp = a.NumType.CompareTo(b.NumType);
        return rankCmp != 0 ? rankCmp : b.SuitType.CompareTo(a.SuitType);
    }
}
