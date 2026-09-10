using GFrameworkTemplate.scripts.constants;
using GFrameworkTemplate.scripts.framework.@event;

namespace GFrameworkTemplate.Tests;

/// <summary>
///     频段事件总线测试。
///     验证：不同频段同名事件互不干扰、同频段多订阅者、取消订阅。
/// </summary>
public class ChannelEventBusTests
{
    /// <summary>
    ///     测试事件（带数据）。
    /// </summary>
    private sealed class TestEvent
    {
        public required string Value { get; init; }
    }

    /// <summary>
    ///     测试标记事件（无数据）。
    /// </summary>
    private sealed class MarkerEvent;

    [Fact]
    public void Send_ToChannel_OnlyNotifiesThatChannelSubscribers()
    {
        var bus = new ChannelEventBus();
        var gameplayReceived = new List<string>();
        var uiReceived = new List<string>();

        bus.RegisterOnChannel<TestEvent>(ChannelConstants.Gameplay, e => gameplayReceived.Add(e.Value));
        bus.RegisterOnChannel<TestEvent>(ChannelConstants.Ui, e => uiReceived.Add(e.Value));

        bus.SendOnChannel(ChannelConstants.Gameplay, new TestEvent { Value = "player-died" });

        Assert.Equal(new[] { "player-died" }, gameplayReceived);
        Assert.Empty(uiReceived);
    }

    [Fact]
    public void Send_ToMultipleChannels_NotifiesRespectiveSubscribers()
    {
        var bus = new ChannelEventBus();
        var gameplayReceived = new List<string>();
        var uiReceived = new List<string>();

        bus.RegisterOnChannel<TestEvent>(ChannelConstants.Gameplay, e => gameplayReceived.Add(e.Value));
        bus.RegisterOnChannel<TestEvent>(ChannelConstants.Ui, e => uiReceived.Add(e.Value));

        bus.SendOnChannel(ChannelConstants.Gameplay, new TestEvent { Value = "a" });
        bus.SendOnChannel(ChannelConstants.Ui, new TestEvent { Value = "b" });

        Assert.Equal(new[] { "a" }, gameplayReceived);
        Assert.Equal(new[] { "b" }, uiReceived);
    }

    [Fact]
    public void SameChannel_MultipleSubscribers_AllReceive()
    {
        var bus = new ChannelEventBus();
        var received1 = new List<string>();
        var received2 = new List<string>();

        bus.RegisterOnChannel<TestEvent>(ChannelConstants.Gameplay, e => received1.Add(e.Value));
        bus.RegisterOnChannel<TestEvent>(ChannelConstants.Gameplay, e => received2.Add(e.Value));

        bus.SendOnChannel(ChannelConstants.Gameplay, new TestEvent { Value = "x" });

        Assert.Equal(new[] { "x" }, received1);
        Assert.Equal(new[] { "x" }, received2);
    }

    [Fact]
    public void UnRegister_StopsReceiving()
    {
        var bus = new ChannelEventBus();
        var received = new List<string>();

        var unReg = bus.RegisterOnChannel<TestEvent>(ChannelConstants.Gameplay, e => received.Add(e.Value));
        bus.SendOnChannel(ChannelConstants.Gameplay, new TestEvent { Value = "before" });

        unReg.UnRegister();
        bus.SendOnChannel(ChannelConstants.Gameplay, new TestEvent { Value = "after" });

        Assert.Equal(new[] { "before" }, received);
    }

    [Fact]
    public void MarkerEvent_NoData_SendsToChannel()
    {
        var bus = new ChannelEventBus();
        var gameplayFired = false;
        var uiFired = false;

        bus.RegisterOnChannel<MarkerEvent>(ChannelConstants.Gameplay, _ => gameplayFired = true);
        bus.RegisterOnChannel<MarkerEvent>(ChannelConstants.Ui, _ => uiFired = true);

        bus.SendOnChannel<MarkerEvent>(ChannelConstants.Gameplay);

        Assert.True(gameplayFired);
        Assert.False(uiFired);
    }

    [Fact]
    public void CustomChannel_Works()
    {
        var bus = new ChannelEventBus();
        var received = new List<string>();

        bus.RegisterOnChannel<TestEvent>("MyCustomChannel", e => received.Add(e.Value));
        bus.SendOnChannel("MyCustomChannel", new TestEvent { Value = "custom" });

        Assert.Equal(new[] { "custom" }, received);
    }

    [Fact]
    public void RuntimeChannelCreation_IsLazy()
    {
        var bus = new ChannelEventBus();
        Assert.Equal(0, bus.ChannelCount);

        bus.SendOnChannel("runtime-channel", new TestEvent { Value = "x" });

        Assert.Equal(1, bus.ChannelCount);
        Assert.True(bus.ContainsChannel("runtime-channel"));
        Assert.Contains("runtime-channel", bus.ChannelNames);
    }

    [Fact]
    public void RemoveChannel_DropsSubscribers_AndRecreatesFresh()
    {
        var bus = new ChannelEventBus();
        var received = new List<string>();
        bus.RegisterOnChannel<TestEvent>("room-1", e => received.Add(e.Value));

        bus.SendOnChannel("room-1", new TestEvent { Value = "first" });
        Assert.Single(received);

        Assert.True(bus.RemoveChannel("room-1"));
        Assert.False(bus.ContainsChannel("room-1"));
        Assert.False(bus.RemoveChannel("room-1"));   // 幂等：移除不存在的频段返回 false

        // 移除后同名频段为全新实例，旧订阅者不再收到（Send 会懒创建新实例）
        bus.SendOnChannel("room-1", new TestEvent { Value = "second" });
        Assert.Single(received);
        Assert.True(bus.ContainsChannel("room-1"));  // 已重建为新实例
    }

    [Fact]
    public void ClearChannels_RemovesAllChannels()
    {
        var bus = new ChannelEventBus();
        bus.SendOnChannel("c1", new TestEvent { Value = "a" });
        bus.SendOnChannel("c2", new TestEvent { Value = "b" });
        Assert.Equal(2, bus.ChannelCount);

        bus.ClearChannels();

        Assert.Equal(0, bus.ChannelCount);
        Assert.Empty(bus.ChannelNames);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void EmptyChannel_Throws(string? channel)
    {
        var bus = new ChannelEventBus();

        // 用 ThrowsAny 兼容 ArgumentNullException（ArgumentException 的派生类型）
        Assert.ThrowsAny<ArgumentException>(() => bus.SendOnChannel(channel!, new TestEvent { Value = "x" }));
        Assert.ThrowsAny<ArgumentException>(() => bus.RegisterOnChannel<TestEvent>(channel!, _ => { }));
    }

    [Fact]
    public void ConcurrentChannelCreation_IsThreadSafe()
    {
        var bus = new ChannelEventBus();

        Parallel.For(0, 200, i => bus.SendOnChannel($"room-{i % 20}", new TestEvent { Value = i.ToString() }));

        Assert.Equal(20, bus.ChannelCount);
    }
}
