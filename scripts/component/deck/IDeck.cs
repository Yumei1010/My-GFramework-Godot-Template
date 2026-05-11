using TimeToTwentyfour.scripts.entities.poker;

namespace TimeToTwentyfour.scripts.component.deck;

/// <summary>
///     牌桌契约
///     负责管理手牌的横向一字布局、拖拽排序与指定条件排序。
/// </summary>
/// <remarks>
///     <code>
///         deck.Add(poker);       // 增加一张手牌
///         deck.Remove(poker);    // 移除一张手牌
///         deck.SortBySuit();     // 按花色排序
///         deck.SortByRank();     // 按点数排序
///     </code>
/// </remarks>
public interface IDeck
{
    /// <summary>
    ///     当前持有的扑克牌列表，顺序即为从左到右的显示顺序。
    /// </summary>
    IList<IPokerView> Pokers { get; }

    /// <summary>
    ///     增加一张手牌到牌桌。
    /// </summary>
    /// <param name="poker">要添加的扑克牌实例。</param>
    void Add(IPokerView poker);

    /// <summary>
    ///     从牌桌移除一张手牌。
    /// </summary>
    /// <param name="poker">要移除的扑克牌实例。</param>
    void Remove(IPokerView poker);

    /// <summary>
    ///     按花色自动排序（♠ &gt; ♥ &gt; ♣ &gt; ♦，同花色按点数升序）。
    /// </summary>
    void SortBySuit();

    /// <summary>
    ///     按点数自动排序（A→K，同点数按花色排序）。
    /// </summary>
    void SortByRank();
}