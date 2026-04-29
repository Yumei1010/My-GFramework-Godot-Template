using TimeToTwentyfour.scripts.entities.poker;

namespace TimeToTwentyfour.scripts.entities.selector;

/// <summary>
///     选择器契约 —— 负责追踪当前有哪些 <see cref="IPoker"/> 处于选中状态。
///     入队采用队列语义（FIFO），弹出采用栈语义（LIFO）。
/// </summary>
/// <remarks>
///     <para>
///         选择器通过订阅 <c>SelectorSelectChangedEvent</c>
///         自动感知牌的选中 / 取消选中。
///     </para>
///     <code>
///         selector.IsSelected(poker); // 查询
///         var last = selector.Pop(); // 栈式弹出（移除最后选中的那张）
///     </code>
/// </remarks>
public interface ISelector
{
    /// <summary>
    ///     当前被选中的扑克列表。
    ///     顺序为选中先后：<c>[0]</c> 最早选中，<c>[^1]</c> 最新选中。
    /// </summary>
    IList<IPoker> Selects { get; }

    /// <summary>
    ///     当前选中数量。
    /// </summary>
    int Count { get; }

    /// <summary>
    ///     选择上限。
    ///     <c>0</c> 表示无上限；<c>&gt; 0</c> 时，新选中超出上限会淘汰最早选中的牌（队首出队）。
    /// </summary>
    int Capacity { get; set; }

    /// <summary>
    ///     判断指定扑克是否已被选中。
    /// </summary>
    /// <param name="poker">待查询的 <see cref="IPoker"/>。</param>
    /// <returns>
    ///     <see langword="true"/> 表示该牌当前处于选择中；
    ///     <see langword="false"/> 表示未选中。
    /// </returns>
    bool IsSelected(IPoker poker);

    /// <summary>
    ///     弹出最后选中的牌（栈顶 / 队尾）。
    /// </summary>
    /// <returns>最后被选中的 <see cref="IPoker"/>；若列表为空则返回 <see langword="null!"/>。</returns>
    /// <remarks>
    ///     弹出后该牌从选中列表中移除。语义为"撤销最近一次选择"。
    /// </remarks>
    IPoker Pop();
}