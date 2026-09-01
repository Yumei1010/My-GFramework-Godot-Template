# 频段事件总线（Channel Event Bus）

`scripts/component/event_bus/` 下的频段事件总线组件，基于 GFramework 的 `EventBus` 扩展，
在"按事件类型分发"之上增加 **频段（Channel）** 维度：**订阅者可以订阅不同频段的同名事件，互不干扰**。

## 为什么需要频段？

同一个事件（例如 `PlayerDiedEvent`），不同系统关心的重点不同：

| 频段 | 关注点 |
|---|---|
| `Gameplay`（游戏逻辑） | 玩家死了 → 结算分数、移除实体 |
| `Ui`（界面） | 玩家死了 → 弹出死亡界面、更新 HUD |
| `Audio`（音频） | 玩家死了 → 播放死亡音效 |
| `Net`（网络） | 玩家死了 → 同步给其他客户端 |

没有频段时：所有订阅者都收同一个事件，靠事件内部字段区分，订阅者要做一堆 `if` 判断。
有频段后：**发到哪个频段，只有那个频段的订阅者收到**，逻辑清晰解耦。

## 快速上手

```csharp
var bus = new ChannelEventBus();

// 订阅：游戏逻辑频段的玩家死亡事件
bus.Register<PlayerDiedEvent>(ChannelConst.Gameplay, e =>
{
    // 只有 Gameplay 频段的事件会到这里
    ScoreManager.AddScore(e.PlayerId);
});

// 订阅：UI 频段的同名事件（互不干扰）
bus.Register<PlayerDiedEvent>(ChannelConst.Ui, e =>
{
    DeathScreen.Show(e.PlayerId);
});

// 发送：只通知 Gameplay 频段的订阅者（UI 频段收不到）
bus.Send(ChannelConst.Gameplay, new PlayerDiedEvent { PlayerId = 1 });

// 无数据事件
bus.Send<GameStartedEvent>(ChannelConst.Gameplay);
```

## 取消订阅

`Register` 返回 `IUnRegister` 句柄，调用 `UnRegister()` 即可注销：

```csharp
var unReg = bus.Register<PlayerDiedEvent>(ChannelConst.Gameplay, handler);
unReg.UnRegister(); // 取消订阅
```

## 自定义频段

`ChannelConst` 是预定义常量，也可直接传字符串自定义频段：

```csharp
bus.Register<SomeEvent>("MyCustomChannel", handler);
bus.Send("MyCustomChannel", new SomeEvent { ... });
```

## 接口契约

`IChannelEventBus`：

| 方法 | 作用 |
|---|---|
| `Register<T>(channel, handler)` | 订阅指定频段的事件，返回取消句柄 |
| `Send<T>(channel, eventData)` | 向指定频段发送带数据的事件 |
| `Send<T>(channel)` | 向指定频段发送无数据事件 |
| `UnRegister<T>(channel, handler)` | 取消指定频段的事件订阅 |

## 实现说明

内部结构：`Dictionary<string, EventBus>` —— 每个频段一个独立的 `EventBus`（GFramework 原生的按类型分发总线）。
`GetOrCreate` 惰性创建频段总线，首次使用时自动建立。

## 测试验证

`tests/` 下的 `ChannelEventBusTests` 覆盖：
- 不同频段同名事件互不干扰
- 同频段多订阅者均收到
- 取消订阅后不再收到
