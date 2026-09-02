# 动作队列（Action Queue）

`scripts/component/action_queue/` 下的动作队列组件：**按序串行执行异步步骤**（前一个完成才执行下一个）。
契合"按序播放动画 / 连锁效果"场景——把步骤排入队列，自动逐个执行。

## 为什么需要？

游戏里常有"连锁反应"：出牌 → 移动动画 → 翻牌 → 计分 → 反馈。
如果用事件直接广播，各步骤会**并行乱序**；用回调嵌套则代码难以维护。
动作队列把步骤排成 FIFO，**自动串行**，每步可 `await` 自己的动画/异步逻辑。

## 快速上手

```csharp
var queue = new ActionQueue();

// 按序排入步骤（可混合同步/异步）
queue.Enqueue(async () => await MoveCardToTarget());  // 第 1 步：移动（await 动画完成）
queue.Enqueue(async () => await FlipCard());           // 第 2 步：翻牌
queue.Enqueue(() => CalculateScore());                 // 第 3 步：计分（同步也可）

// 队列自动串行：移动完 → 翻牌完 → 计分
// 运行中再 Enqueue 会自动追加到队尾，等前序完成
```

## 核心 API

| 成员 | 作用 |
|---|---|
| `Enqueue(Func<Task> step)` | 排入一步。队列空闲立即开始，否则等前序 |
| `WaitUntilIdleAsync()` | 等待当前及后续所有步骤执行完毕（测试/接续用） |
| `Clear()` | 清空待执行步骤（正在执行的不受影响） |
| `IsRunning` | 是否正在执行 |
| `PendingCount` | 待执行步骤数（不含正在执行的） |
| `IsEmpty` | 是否完全空闲 |

## 在动画系统中的应用（Godot）

配合 Godot 协程/信号等待，实现"按序播放"：

```csharp
var queue = new ActionQueue();

// 步骤里 await 动画完成信号
queue.Enqueue(async () =>
{
    var tween = GetNode<Control>("%Card").CreateTween();
    tween.TweenProperty(GetNode<Control>("%Card"), "position", targetPos, 0.3);
    await ToSignal(tween, Tween.SignalName.Finished);
});

queue.Enqueue(async () =>
{
    // 或触发 CQRS 事件后等对应完成事件
    this.SendEvent(new PlayFlipAnimEvent { CardId = id });
    await ToSignal(this, AnimationFinishedSignal);  // 等动画系统回执
});
```

## 注意事项

- 纯逻辑组件，零 Godot/GFramework 依赖，可在单元测试中使用
- 步骤异常会中断队列（`finally` 保证状态复位）；如需容错请在步骤内自行 try/catch
- 队列按入队顺序执行；`Clear()` 只清"未开始的"，运行中的步骤会自然结束
- 单线程场景为主（Godot 主线程）；跨线程入队需自行加锁

## 测试验证

`tests/` 下的 `ActionQueueTests` 覆盖：
- 串行顺序（A 完全结束才 B 开始）
- 运行中入队追加
- 空闲后再次入队立即执行
- Clear 丢弃待执行步骤
- 完成状态正确复位
