# 协程系统教程（Coroutine Scheduler）

> 本文纯教学：讲清"为什么需要 GFramework 协程 + 怎么用"。
> 框架已带完整实现（`CoroutineScheduler` + 20 个等待指令 + CQRS 集成），无需造轮子。
> 模板已部分使用（`SceneTransitionManager` 转场协程 + `Timing.Prewarm()`）。

---

## 0. 一句话先说明白

**协程 = 把"分步执行的游戏流程"写成顺序代码**：每一步 `yield return` 一个等待指令，调度器按时间/帧/条件推进，暂停/恢复/取消由句柄控制。

它比 Godot 原生 `await ToSignal` 强在：**可统计、可快照、有统一异常、能等 C# Task、还能在协程里发 CQRS 命令并等事件**。

---

## 1. 为什么需要（对比三种写法）

| 写法 | 问题 |
|---|---|
| 回调嵌套 | 流程一长就"回调地狱"，顺序逻辑支离破碎 |
| Godot `await ToSignal` | 无法等待"命令完成""事件到达"，无统一异常处理，无法统计活跃协程 |
| **GFramework 协程** | 顺序化编排 + CQRS 集成 + 可观测性 |

典型场景：转场（遮罩→切场景→恢复）、关卡流程（发命令→等结果→下一步）、对话序列（打字机→等玩家→下一句）、进度条动画。

---

## 2. 核心概念

```
IEnumerator<IYieldInstruction>  ← 你写的协程（yield return 等待指令）
        │ RunCoroutine()
        ▼
CoroutineScheduler（Timing 静态入口）   ← 推进每个活跃协程
        │ 每帧 Update(deltaTime)
        ▼
每个 IYieldInstruction 判断 IsDone → 未完继续等，完成前进下一步
```

### 等待指令（IYieldInstruction）

```csharp
public interface IYieldInstruction
{
    bool IsDone { get; }
    void Update(double deltaTime);   // 调度器每帧调用
}
```

**yield 什么 = 等什么。** `yield return new WaitForSecondsScaled(2.0)` = 等 2 秒（受 TimeScale 影响）。

---

## 3. 启动方式（三种）

```csharp
using GFramework.Godot.Coroutine;   // Godot 扩展
using GFramework.Core.Coroutine;    // Timing

// 方式 A：节点扩展 RunCoroutine —— 节点销毁自动停止（最常用）
public override void _Ready()
{
    this.RunCoroutine(FlowCoroutine());   // this 是 Godot 节点
}

// 方式 B：Timing 静态入口（全局）
Timing.RunGameCoroutine(FlowCoroutine());   // 游戏阶段
Timing.RunUiCoroutine(FlowCoroutine());     // UI 阶段

// 方式 C：owned（归属某节点，带取消令牌）
this.RunCoroutine(FlowCoroutine(), tag: "main_flow",
    cancellationToken: someToken);
```

**节点扩展的隐藏能力**：`RunCoroutine(Node owner, ...)` 会自动在节点销毁时停止协程；`CancelWith(node)` 包装器可让协程在节点死亡时提前结束。

### Segment（运行阶段）

`Segment.Process` / `PhysicsProcess` 等——决定协程在哪个帧阶段推进（默认 Process）。

---

## 4. 20 个等待指令速查

| 类别 | 指令 | 说明 |
|---|---|---|
| 时间 | `WaitForSecondsScaled(sec)` | 等 N 秒（受 TimeScale，暂停/慢动作会停/变慢） |
| 时间 | `WaitForSecondsRealtime(sec)` | 等 N 秒（不受 TimeScale，暂停也走） |
| 时间 | `WaitForProgress(dur, onProgress)` | 走完 duration，期间回调进度 0→1（进度条动画） |
| 帧 | `WaitForNextFrame` / `WaitForFrames(n)` | 等一帧 / N 帧 |
| 帧 | `WaitForEndOfFrame` / `WaitForFixedUpdate` | 帧末 / 物理帧 |
| 条件 | `WaitUntil(predicate)` / `WaitWhile(predicate)` | 等条件成立 / 等条件消失 |
| 条件 | `WaitUntilOrTimeout(pred, timeout)` | 等条件或超时（返回结果可判哪个先到） |
| 条件 | `WaitForConditionChange(getter)` | 等某值变化 |
| 事件 | `WaitForEvent<TEvent>()` | 等 CQRS 事件（`RegisterEvent` 桥接） |
| 事件 | `WaitForMultipleEvents(...)` | 等多个事件 |
| 事件 | `WaitForEventWithTimeout<...>(timeout)` | 等事件或超时 |
| Task | `WaitForTask(task)` / `WaitForTask<T>(task)` | **等 C# Task**——纯 async/await 与协程互桥 |
| 协程 | `WaitForCoroutine(other)` | 等另一个协程完成 |
| 协程 | `WaitForAllCoroutines(...)` | 等一批协程全部完成 |

---

## 5. CoroutineHelper（无脑工厂）

`GFramework.Core.Coroutine.CoroutineHelper` 提供常用指令/协程：

```csharp
using GFramework.Core.Coroutine;

yield return CoroutineHelper.WaitForSeconds(1.5);      // 等 1.5s
yield return CoroutineHelper.WaitForOneFrame();
yield return CoroutineHelper.WaitUntil(() => canGo);   // 等条件
yield return CoroutineHelper.WaitForProgress(2.0, p => bar.Value = p);  // 进度条

// 直接产出一条协程：
IEnumerator<IYieldInstruction> c = CoroutineHelper.DelayedCall(1.0, () => GD.Print("延迟1秒"));
IEnumerator<IYieldInstruction> r = CoroutineHelper.RepeatCall(0.5, 3, () => Tick());  // 每0.5s×3次
IEnumerator<IYieldInstruction> f = CoroutineHelper.RepeatCallForever(1.0, () => ...); // 无限循环
```

---

## 6. 🔥 CQRS 集成（模板最可能用上的）

这是这套协程最特别的地方——**在协程里直接编排命令/事件**：

```csharp
using GFramework.Core.Coroutine.Extensions;   // 这些扩展方法

public IEnumerator<IYieldInstruction> RoundFlow()
{
    _log.Info("回合开始");

    // 发命令 + 等到指定事件（发 start_round 命令，等 RoundStartedEvent 再继续）
    yield return this.SendCommandAndWaitEventCoroutine<
        StartRoundCommand, RoundStartedEvent>();

    // 等待玩家操作阶段（等事件：玩家提交答案）
    yield return this.WaitForEvent<PlayerAnsweredEvent>();

    // 编辑分发伤害命令，带错误处理
    yield return this.SendCommandCoroutineWithErrorHandler<ApplyDamageCommand>(
        onError: ex => _log.Error($"命令失败: {ex}"));

    _log.Info("回合结束");
}
```

```csharp
// 具体签名：
SendCommandCoroutineWithErrorHandler<TCommand>(
    onError: Action<Exception>? = null)

SendCommandAndWaitEventCoroutine<TCommand, TEvent>(
    timeout: TimeSpan? = null)   // 可传超时
```

**效果**：关卡/回合/结算这类"流程"变成顺序代码，不用嵌套回调，也不用到处 `await ToSignal`。

---

## 7. 组合与编排

```csharp
using GFramework.Core.Coroutine.Extensions;

// 顺序连接（Then）
var flow = CoroutineHelper.WaitForSeconds(1.0)
    .Then(SendCommandAndWaitEventCoroutine<Cmd, Evt>())
    .Then(CoroutineHelper.WaitForProgress(1.0, p => fade.Alpha = p));

// 并行（ParallelCoroutines）—— 等全部完成
var both = ParallelCoroutines(
    CoroutineHelper.WaitForSeconds(1.0),
    CoroutineHelper.WaitForSeconds(2.0));

// 周期执行（RepeatEvery）
yield return RepeatEvery(0.5, () => UpdateTimer(), count: 10);
```

---

## 8. 控制与可观测性

```csharp
// 启动拿到句柄
CoroutineHandle h = this.RunCoroutine(Flow());

// 控制
Timing.PauseCoroutine(h);      // 暂停
Timing.ResumeCoroutine(h);     // 恢复

// 可观测性
Timing.Instance.ActiveCoroutineCount;              // 活跃数（查泄漏）
Timing.Instance.IsCoroutineAlive(h);
Timing.Instance.TryGetSnapshot(h, out CoroutineSnapshot s);  // 单条快照
Timing.Instance.GetActiveSnapshots();              // 全部快照
Timing.Instance.WaitForCompletionAsync(h);         // await 它完成（状态）

// 事件
Timing.Instance.OnCoroutineException += (_, e) => _log.Error(e.Exception.ToString());
Timing.Instance.OnCoroutineFinished += (_, e) => _log.Debug($"完成: {e.Handle}");
```

---

## 9. 错误处理

协程内未捕获异常会被调度器捕获并触发 `OnCoroutineException`（不崩溃游戏），默认继续。建议注册统一处理器记录日志。

---

## 10. 模板接入方式

模板已接好**调度器预热**（`GameEntryPoint` 调 `Timing.Prewarm()`）。

业务侧直接用 Node 扩展即可（`SceneTransitionManager` 已示范）：

```csharp
[Log]
[ContextAware]
public partial class RoundFlow : Node
{
    private CoroutineHandle _handle;

    public override void _Ready()
    {
        _handle = this.RunCoroutine(Flow());   // 节点销毁自动停
    }

    private IEnumerator<IYieldInstruction> Flow()
    {
        yield return CoroutineHelper.WaitForSeconds(1.0);
        yield return this.SendCommandAndWaitEventCoroutine<StartCmd, StartedEvt>();
        _log.Info("流程完成");
    }
}
```

---

## 11. 常见坑

| 坑 | 说明 |
|---|---|
| **yield 忘 return** | 协程内必须 `yield return`，否则编译器报错（或解析为普通方法） |
| **节点销毁后协程仍跑** | 用节点扩展 `RunCoroutine(this Node, ...)`（自动停）或 `CancelWith(node)`，别用裸 `RunCoroutine()` |
| **饿了忘错误处理** | 协程异常不崩游戏但会静默丢，务必注册 `OnCoroutineException` |
| **TimeScale 语义** | `WaitForSecondsScaled` 受 GameTimeScale；`Realtime` 不受；选错会暂停时走秒 |
| **重复启动** | 同流程重复 Run 会并行跑两份，用 tag 或句柄管理防重 |

---

## 12. 什么时候用 / 不用

| 情况 | 选择 |
|---|---|
| 转场/回合/对话/引导这类**分步流程** | ✅ 协程 |
| "发命令→等结果→继续"的时序 | ✅ 协程 + CQRS 集成 |
| 纯计算、无等待逻辑 | ❌ 普通方法即可 |
| 简单 `await` 单个信号 | Godot `await` 也行，但无统计/异常统一 |

---

## 13. API 速查

| API | 说明 |
|---|---|
| `this.RunCoroutine(enum, segment?, tag?)` / `Timing.RunGameCoroutine` / `RunUiCoroutine` | 启动（节点扩展自动随节点销毁停止） |
| `IYieldInstruction`（`IsDone` / `Update(delta)`) | 等待指令接口 |
| `CoroutineHelper.*` | 指令/协程工厂 |
| `Commands` 扩展：`SendCommandAndWaitEventCoroutine<T,TEvent>` / `SendCommandCoroutineWithErrorHandler<T>` | CQRS 协程集成 |
| `Then` / `ParallelCoroutines` / `Sequence` / `RepeatEvery` | 组合编排 |
| `Timing.Pause/ResumeCoroutine(handle)` | 暂停/恢复 |
| `WaitForCompletionAsync` / `GetActiveSnapshots` / `ActiveCoroutineCount` | 可观测性 |
| `OnCoroutineException` / `OnCoroutineFinished` | 统一事件 |

---

## 14. 源码阅读入口

| 路径 | 内容 |
|---|---|
| `GFramework.Core/Coroutine/CoroutineScheduler.cs` | 调度核心（推进/句柄/统计/组/标签） |
| `GFramework.Godot/Coroutine/Timing.cs` | Godot 静态入口（Run/Pause/Resume/快照）+ 预热 |
| `GFramework.Godot/Coroutine/CoroutineNodeExtensions.cs` | 节点扩展（owned 协程自动停） |
| `GFramework.Core/Coroutine/Instructions/` | 20 个等待指令 |
| `GFramework.Core/Coroutine/Extensions/` | CQRS/组合/周期扩展 |
| `GFramework.Core.Abstractions/Coroutine/` | 接口（IYieldInstruction 等） |
| `docs/zh-CN/core/coroutine.md` | 框架官方文档 |