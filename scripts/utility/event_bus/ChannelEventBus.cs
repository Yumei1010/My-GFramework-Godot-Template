using GFramework.Core.Abstractions.events;
using GFramework.Core.events;

namespace GFrameworkTemplate.scripts.utility.event_bus;

/// <summary>
///     频段事件总线实现。
///     内部为每个频段维护一个独立的 <see cref="EventBus"/>，按"频段 + 事件类型"分发：
///     同名事件发到不同频段互不干扰，订阅者只收到自己订阅频段的事件。
/// </summary>
/// <remarks>
///     用法示例：
///     <code>
///     var bus = new ChannelEventBus();
///
///     // 订阅"游戏逻辑"频段的玩家死亡事件
///     var unReg = bus.Register&lt;PlayerDiedEvent&gt;(ChannelConst.Gameplay, e =&gt; { ... });
///
///     // 向"游戏逻辑"频段发送（只通知该频段订阅者）
///     bus.Send(ChannelConst.Gameplay, new PlayerDiedEvent { PlayerId = 1 });
///
///     unReg.UnRegister(); // 取消订阅
///     </code>
/// </remarks>
public sealed class ChannelEventBus : IChannelEventBus
{
    private readonly Dictionary<string, EventBus> _channels = new(StringComparer.Ordinal);

    /// <inheritdoc />
    public IUnRegister Register<T>(string channel, Action<T> handler)
    {
        return GetOrCreate(channel).Register(handler);
    }

    /// <inheritdoc />
    public void Send<T>(string channel, T eventData)
    {
        GetOrCreate(channel).Send(eventData);
    }

    /// <inheritdoc />
    public void Send<T>(string channel) where T : new()
    {
        GetOrCreate(channel).Send(new T());
    }

    /// <inheritdoc />
    public void UnRegister<T>(string channel, Action<T> handler)
    {
        GetOrCreate(channel).UnRegister(handler);
    }

    /// <summary>
    ///     获取指定频段的 EventBus，不存在则创建。
    /// </summary>
    /// <param name="channel">频段名称</param>
    /// <returns>该频段对应的 EventBus</returns>
    private EventBus GetOrCreate(string channel)
    {
        if (!_channels.TryGetValue(channel, out var bus))
        {
            bus = new EventBus();
            _channels[channel] = bus;
        }

        return bus;
    }
}
