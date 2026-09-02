using GFramework.Core.Abstractions.events;
using GFramework.Core.Abstractions.utility;

namespace GFrameworkTemplate.scripts.utility.event_bus;

/// <summary>
///     频段事件总线接口契约。
///     在框架原有事件总线的基础上增加"频段（Channel）"维度：
///     订阅者订阅某个频段上的事件，只收到该频段发出的同名事件，不同频段互不干扰。
///     作为架构 Utility 注册（见 <c>UtilityModule</c>），通过 <c>GetUtility&lt;IChannelEventBus&gt;()</c> 获取。
/// </summary>
/// <example>
///     同一事件 <c>PlayerDiedEvent</c> 可以发到不同频段：
///     <code>
///     // 游戏逻辑频段：战斗系统订阅
///     channelBus.Register&lt;PlayerDiedEvent&gt;(ChannelConst.Gameplay, e =&gt; { ... });
///     // UI 频段：界面订阅
///     channelBus.Register&lt;PlayerDiedEvent&gt;(ChannelConst.Ui, e =&gt; { ... });
///
///     // 只通知游戏逻辑频段（UI 频段的订阅者收不到）
///     channelBus.Send(ChannelConst.Gameplay, new PlayerDiedEvent { ... });
///     </code>
/// </example>
public interface IChannelEventBus : IUtility
{
    /// <summary>
    ///     订阅指定频段上的事件。同一频段、同一事件类型可多个订阅者，均会收到。
    /// </summary>
    /// <typeparam name="T">事件类型</typeparam>
    /// <param name="channel">频段名称</param>
    /// <param name="handler">事件处理回调</param>
    /// <returns>取消订阅句柄，调用 <see cref="IUnRegister.UnRegister"/> 可注销</returns>
    IUnRegister Register<T>(string channel, Action<T> handler);

    /// <summary>
    ///     向指定频段发送事件（带数据）。
    /// </summary>
    /// <typeparam name="T">事件类型</typeparam>
    /// <param name="channel">频段名称</param>
    /// <param name="eventData">事件数据</param>
    void Send<T>(string channel, T eventData);

    /// <summary>
    ///     向指定频段发送事件（无数据标记事件）。
    /// </summary>
    /// <typeparam name="T">事件类型（需有公共无参构造函数）</typeparam>
    /// <param name="channel">频段名称</param>
    void Send<T>(string channel) where T : new();

    /// <summary>
    ///     取消指定频段上的事件订阅。
    /// </summary>
    /// <typeparam name="T">事件类型</typeparam>
    /// <param name="channel">频段名称</param>
    /// <param name="handler">要取消的事件处理回调</param>
    void UnRegister<T>(string channel, Action<T> handler);
}
