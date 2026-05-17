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
    // TODO: 补充方法声明 —— IsSelected(IPoker)、Pop()、Count
}