# 频段事件总线（Channel Event Bus）

`scripts/utility/event/` 下的频段事件总线，**集成进框架原版事件总线体系**（`RegisterEvent` / `SendEvent`），
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

## 快速上手（框架风格）

在任意 `[ContextAware]` 节点中，与框架 API 完全一致，只是多一个频段参数：

```csharp
// 订阅：游戏逻辑频段的玩家死亡事件
this.RegisterEvent<PlayerDiedEvent>(ChannelConstants.Gameplay, e =>
{
    // 只有 Gameplay 频段的事件会到这里
    ScoreManager.AddScore(e.PlayerId);
});

// 订阅：UI 频段的同名事件（互不干扰）
this.RegisterEvent<PlayerDiedEvent>(ChannelConstants.Ui, e =>
{
    DeathScreen.Show(e.PlayerId);
});

// 发送：只通知 Gameplay 频段的订阅者（UI 频段收不到）
this.SendEvent(ChannelConstants.Gameplay, new PlayerDiedEvent { PlayerId = 1 });

// 无数据事件
this.SendEvent<GameStartedEvent>(ChannelConstants.Gameplay);
```

> 无频段的 `this.RegisterEvent<T>(e => ...)` / `this.SendEvent(new T{...})` 仍是框架原版，两者重载共存互不影响。

## 取消订阅

`RegisterEvent` 返回 `IUnRegister` 句柄，调用 `UnRegister()` 即可注销：

```csharp
var unReg = this.RegisterEvent<PlayerDiedEvent>(ChannelConstants.Gameplay, handler);
unReg.UnRegister(); // 取消订阅
```

## 自定义频段

`ChannelConstants` 是预定义常量，也可直接传字符串自定义频段：

```csharp
this.RegisterEvent<SomeEvent>("MyCustomChannel", handler);
this.SendEvent("MyCustomChannel", new SomeEvent { ... });
```

## 架构集成（GFramework 0.7.1+）

| 组件 | 作用 |
|---|---|
| `ChannelEventBus` | **继承原版 `EventBus`**，新增频段能力（`RegisterOnChannel` / `SendOnChannel` / `SendOnChannel<T>`），天然实现 `IEventBus` |
| `ContextAwareChannelExtensions` | 为 `IContextAware` 扩展 `RegisterEvent<T>(channel, ...)` / `SendEvent(channel, ...)`，与框架 API 风格一致 |
| `ChannelConstants` | 预定义频段（`Gameplay` / `Ui` / `Audio` / `Net`） |

### 如何生效（关键）

`GameArchitecture.Configurator` 用 `ChannelEventBus` **覆盖容器中的 `IEventBus`**：

```csharp
public override Action<IServiceCollection>? Configurator =>
    services =>
    {
        var bus = new ChannelEventBus();
        services.AddSingleton(bus);                    // 具体类型（扩展方法 GetService 用）
        services.AddSingleton(typeof(IEventBus), bus); // 接口（原版 RegisterEvent/SendEvent 用）
    };
```

升级到 0.7.1 后，框架的 `ArchitectureServices.EventBus => Container.Get<IEventBus>()` 动态获取，
因此**原版 `RegisterEvent` / `SendEvent` 直接走频段总线**（不替换时行为不变）。

## 实现说明

`ChannelEventBus` 继承原版 `EventBus`，内部为每个频段维护一个独立的 `EventBus` 实例
（`Dictionary<string, EventBus>`），按"频段 + 事件类型"分发。`GetChannel` 惰性创建频段总线。

## 测试验证

`tests/` 下的 `ChannelEventBusTests` 覆盖：
- 不同频段同名事件互不干扰
- 同频段多订阅者均收到
- 取消订阅后不再收到
- 无数据标记事件、自定义频段
