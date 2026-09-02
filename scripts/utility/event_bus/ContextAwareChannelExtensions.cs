using GFramework.Core.Abstractions.events;
using GFramework.Core.Abstractions.rule;
using GFramework.Core.extensions;

namespace GFrameworkTemplate.scripts.utility.event_bus;

/// <summary>
///     频段事件总线扩展方法：把频段事件收发集成进框架原本的 <c>RegisterEvent</c> / <c>SendEvent</c> 体系。
///     带频段的重载与框架原版（<c>ContextAwareExtensions.RegisterEvent</c> / <c>SendEvent</c>）重载共存：
///     <list type="bullet">
///         <item><description><c>this.RegisterEvent&lt;T&gt;(e =&gt; ...)</c> → 框架原版（无频段）</description></item>
///         <item><description><c>this.RegisterEvent&lt;T&gt;(channel, e =&gt; ...)</c> → 本扩展（频段版）</description></item>
///     </list>
/// </summary>
/// <example>
///     <code>
///     // 在任意 [ContextAware] 节点中：
///     this.RegisterEvent&lt;PlayerDiedEvent&gt;(ChannelConst.Gameplay, e =&gt; { ... });
///     this.SendEvent(ChannelConst.Gameplay, new PlayerDiedEvent { PlayerId = 1 });
///     this.SendEvent&lt;GameStartedEvent&gt;(ChannelConst.Gameplay); // 无数据事件
///     </code>
/// </example>
public static class ContextAwareChannelExtensions
{
    /// <summary>
    ///     订阅指定频段上的事件（框架事件总线的频段扩展）。
    /// </summary>
    /// <typeparam name="TEvent">事件类型</typeparam>
    /// <param name="contextAware">上下文感知对象（Godot 节点 / 状态等）</param>
    /// <param name="channel">频段名称（见 <see cref="ChannelConst"/>）</param>
    /// <param name="handler">事件处理回调</param>
    /// <returns>取消订阅句柄，调用 <see cref="IUnRegister.UnRegister"/> 可注销</returns>
    public static IUnRegister RegisterEvent<TEvent>(this IContextAware contextAware, string channel, Action<TEvent> handler)
    {
        return contextAware.GetContext().GetUtility<IChannelEventBus>()!.Register(channel, handler);
    }

    /// <summary>
    ///     向指定频段发送事件（带数据，框架事件总线的频段扩展）。
    /// </summary>
    /// <typeparam name="TEvent">事件类型</typeparam>
    /// <param name="contextAware">上下文感知对象</param>
    /// <param name="channel">频段名称（见 <see cref="ChannelConst"/>）</param>
    /// <param name="eventData">事件数据</param>
    public static void SendEvent<TEvent>(this IContextAware contextAware, string channel, TEvent eventData)
    {
        contextAware.GetContext().GetUtility<IChannelEventBus>()!.Send(channel, eventData);
    }

    /// <summary>
    ///     向指定频段发送事件（无数据标记事件，框架事件总线的频段扩展）。
    /// </summary>
    /// <typeparam name="TEvent">事件类型（需有公共无参构造函数）</typeparam>
    /// <param name="contextAware">上下文感知对象</param>
    /// <param name="channel">频段名称（见 <see cref="ChannelConst"/>）</param>
    public static void SendEvent<TEvent>(this IContextAware contextAware, string channel) where TEvent : new()
    {
        contextAware.GetContext().GetUtility<IChannelEventBus>()!.Send<TEvent>(channel);
    }
}
