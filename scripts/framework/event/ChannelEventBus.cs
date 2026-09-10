using System.Collections.Concurrent;
using GFramework.Core.Abstractions.Events;
using GFramework.Core.Events;

namespace GFrameworkTemplate.scripts.framework.@event;

/// <summary>
///     频段事件总线实现：**继承框架原版 <see cref="EventBus" />**，
///     在保留原版按类型分发能力的同时，增加"频段（Channel）"维度：
///     同名事件发到不同频段互不干扰，订阅者只收到自己订阅频段的事件。
/// </summary>
/// <remarks>
///     <para>
///         继承原版 EventBus 使其天然实现 <c>IEventBus</c>，可通过架构 <c>Configurator</c>
///         覆盖容器中的 IEventBus 注册，让原版 RegisterEvent / SendEvent 直接支持频段。
///     </para>
///     <para>
///         <b>频段是懒创建的</b>：向任意名称发送/订阅即自动创建该频段，因此运行时可自由创建新频段。
///         但频段会一直驻留内存，**动态频段（如每局/每房间一个名称）必须在使用完毕后调用
///         <see cref="RemoveChannel" /> 释放**，否则会持续累积；固定用途建议改用
///         <c>ChannelConstants</c> 中的常量频段。
///     </para>
///     <para>
///         实现使用 <see cref="ConcurrentDictionary{TKey,TValue}" />，可跨线程安全地创建/发送/移除频段。
///     </para>
/// </remarks>
public class ChannelEventBus : EventBus
{
    private readonly ConcurrentDictionary<string, EventBus> _channels = new(StringComparer.Ordinal);

    /// <summary>
    ///     当前已创建的频段名称集合（用于调试/诊断）。
    /// </summary>
    public IReadOnlyCollection<string> ChannelNames => _channels.Keys.ToArray();

    /// <summary>
    ///     当前已创建的频段数量。
    /// </summary>
    public int ChannelCount => _channels.Count;

    /// <summary>
    ///     订阅指定频段上的事件。
    /// </summary>
    /// <typeparam name="T">事件类型</typeparam>
    /// <param name="channel">频段名称</param>
    /// <param name="handler">事件处理回调</param>
    /// <returns>取消订阅句柄</returns>
    /// <exception cref="ArgumentException">当 <paramref name="channel" /> 为空时抛出。</exception>
    public IUnRegister RegisterOnChannel<T>(string channel, Action<T> handler)
    {
        ArgumentException.ThrowIfNullOrEmpty(channel);
        ArgumentNullException.ThrowIfNull(handler);

        return GetChannel(channel).Register(handler);
    }

    /// <summary>
    ///     向指定频段发送事件（带数据）。
    /// </summary>
    /// <typeparam name="T">事件类型</typeparam>
    /// <param name="channel">频段名称</param>
    /// <param name="eventData">事件数据</param>
    /// <exception cref="ArgumentException">当 <paramref name="channel" /> 为空时抛出。</exception>
    public void SendOnChannel<T>(string channel, T eventData)
    {
        ArgumentException.ThrowIfNullOrEmpty(channel);
        GetChannel(channel).Send(eventData);
    }

    /// <summary>
    ///     向指定频段发送事件（无数据标记事件）。
    /// </summary>
    /// <typeparam name="T">事件类型（需有公共无参构造函数）</typeparam>
    /// <param name="channel">频段名称</param>
    /// <exception cref="ArgumentException">当 <paramref name="channel" /> 为空时抛出。</exception>
    public void SendOnChannel<T>(string channel) where T : new()
    {
        ArgumentException.ThrowIfNullOrEmpty(channel);
        GetChannel(channel).Send<T>();
    }

    /// <summary>
    ///     判断指定频段是否已创建。
    /// </summary>
    /// <param name="channel">频段名称</param>
    /// <returns>该频段已存在时返回 true。</returns>
    public bool ContainsChannel(string channel)
    {
        return !string.IsNullOrEmpty(channel) && _channels.ContainsKey(channel);
    }

    /// <summary>
    ///     移除指定频段（连同其上的全部订阅）。
    /// </summary>
    /// <param name="channel">频段名称</param>
    /// <returns>成功移除返回 true；频段不存在返回 false。</returns>
    /// <remarks>
    ///     移除后该频段名再次被使用时将创建全新的频段实例，因此旧订阅者不会再收到新事件
    ///     （其持有的取消句柄仍指向已丢弃的实例，调用无副作用）。
    ///     动态频段使用完毕后应调用本方法释放，避免内存持续累积。
    /// </remarks>
    public bool RemoveChannel(string channel)
    {
        return !string.IsNullOrEmpty(channel) && _channels.TryRemove(channel, out _);
    }

    /// <summary>
    ///     移除全部频段（连同其上的全部订阅）。
    /// </summary>
    public void ClearChannels()
    {
        _channels.Clear();
    }

    /// <summary>
    ///     获取指定频段的 EventBus，不存在则创建。
    /// </summary>
    /// <param name="channel">频段名称（已由调用方校验非空）</param>
    /// <returns>该频段对应的 EventBus</returns>
    private EventBus GetChannel(string channel)
    {
        return _channels.GetOrAdd(channel, static _ => new EventBus());
    }
}
