# Store 状态管理教程（Redux 风格）

> 本文纯教学：讲清"为什么需要 Store + 怎么用 GFramework 原生 Store 系统"。
> 框架已带完整实现（`Store<TState>`，1500+ 行），无需自造轮子。
> 前置知识：读完 [对象池教程](object-pool.md) 那类风格即可；本文从零讲起，不需要 Redux 背景。

---

## 0. 一句话先说明白

**Store = 把散落各处、随手可改的状态，收编成"一份不可变状态 + 唯一提交入口 + 纯函数规则 + 订阅通知"。**

它解决的是这样一类问题：**一个操作要同时改好几个字段，且多个界面都要跟着更新。**

---

## 1. 为什么需要它（先看老写法的痛）

假设你的游戏 HUD 要显示：分数 / 生命 / 关卡 / 是否通关。

不用 Store 时，代码通常长这样：

```csharp
// 过关了，手动改一大堆
score += 100;
level += 1;
if (level >= 5) isCleared = true;

// 然后必须"想起来"同步给所有关心的人
scoreLabel.Text = score.ToString();
levelLabel.Text = level.ToString();
if (isCleared) ShowVictoryPanel();
saveService.Save(score, level, isCleared);   // 存档也别忘了
```

问题一个接一个：

| 痛点 | 具体表现 |
|---|---|
| **漏同步就出 bug** | 新增"最高分记录"时忘在这里更新 → 分数对但最高分不动 |
| **重复代码到处飞** | 失败扣血的地方，又是另一份类似的同步代码 |
| **无法撤销** | 玩家想悔棋，你根本不知道上一关的分数 |
| **无法追查** | 玩家报"分数不对"，你没有操作记录，查不了 |
| **改起来牵一发动全身** | 加一个新字段，要满项目找"哪里该同步" |

Store 把这堆东西 **收成一个入口**：过关时只调一行 `store.Dispatch(new RoundWonAction(100))`，剩下的自动完成。

---

## 2. 核心概念：四个角色

用"游戏存档"来理解最直观：

```
玩家做了动作（过关）
      │
      │  ① 写一张"事件单"： RoundWon(Bonus: 100)
      ▼
 [Dispatch 提交]
      │
      ▼
② 规则引擎（Reducer）：拿【旧存档】+【事件单】→ 算出【新存档】
      │
      ▼
③ 新存档替换旧存档（旧的不涂改，是"另存为一份"）
      │
      ▼
④ 所有"订阅了存档"的人醒来，各自读新存档刷新自己
```

| 词 | 中文 | 是什么 |
|---|---|---|
| **State** | 状态 | 游戏存档（一整块不可变数据） |
| **Action** | 动作 / 事件单 | 描述"刚刚发生了什么"的纯数据（如 `RoundWonAction`） |
| **Reducer** | 归约器 / 规则书 | 一个函数：`(旧状态, 动作) → 新状态` |
| **Dispatch** | 派发 / 提交 | 把动作递交进 Store 的动作 |
| **Subscribe** | 订阅 | 登记"状态变了通知我" |

**Reducer 不是魔法**，就是个普通函数：

```csharp
// 输入旧状态 + 事件单，输出新状态
RunState Reduce(RunState oldState, RoundWonAction action)
{
    return new RunState(
        Score: oldState.Score + action.Bonus,   // 规则：加分
        Level: oldState.Level + 1,              // 规则：关卡+1
        Hp: oldState.Hp,
        IsCleared: oldState.Level + 1 >= 5
    );
}
```

**三条铁律**：
1. State 不可变（用 `record`）
2. Reducer 是纯函数（同样输入永远同样输出，不碰 IO、不改外部）
3. 改状态的唯一入口是 `Dispatch`

---

## 3. 关键理解：什么叫"不可变"

**不可变 = 不修改旧数据，只造一份新的。**

```csharp
// ❌ 可变写法：把旧数据涂改了，上一刻的分数再也找不回来
state.Score += 100;

// ✅ 不可变写法：复制一份，只改 Score，旧的原封不动
var newState = state with { Score = state.Score + 100 };
```

`record` + `with` 是 C# 语法糖：**复制出一个新对象，指定字段用新值，其余字段照抄**。

为什么非要这么麻烦？想象成**账本**：

- 可变写法 = 直接在账本上**涂改**数字 → 涂完就不知道原来是多少
- 不可变写法 = **新开一页**，抄下旧数字、改掉要改的 → 所有旧页都留着

带来的好处：

| 好处 | 说明 |
|---|---|
| **可撤销** | 旧页都还在，`Undo()` 就是翻回上一页 |
| **可追溯** | 每次变化对应一张事件单，能回放、能调试 |
| **可测试** | reducer 是纯函数，喂输入看输出，不用启动游戏 |
| **线程安全** | 别人拿到的旧状态不会被偷偷改掉 |

> 这也是为什么教程反复强调"不可变"——如果状态能被涂改，历史记录就毫无意义了。

---

## 4. 五分钟上手（最小完整例子）

### 第 1 步：定义状态和动作（都用 record）

```csharp
namespace GFrameworkTemplate.scripts.model.run;

/// <summary>Run 的聚合状态（不可变）。</summary>
public sealed record RunState(int Score, int Hp, int Level, bool IsCleared);

/// <summary>过关事件单。</summary>
public sealed record RoundWonAction(int Bonus);

/// <summary>失败事件单。</summary>
public sealed record RoundLostAction();
```

> 放哪：模板里状态可以放 `scripts/model/<领域>/` 下（和 Model 同域）。

### 第 2 步：建立 Store 并注册规则

```csharp
using GFramework.Core.StateManagement;

var store = new Store<RunState>(new RunState(Score: 0, Hp: 3, Level: 1, IsCleared: false))
    // 过关：分数+、关卡+1、可能通关（一次动作协同改 3 个字段）
    .RegisterReducer<RoundWonAction>((state, action) => state with
    {
        Score = state.Score + action.Bonus,
        Level = state.Level + 1,
        IsCleared = state.Level + 1 >= 5
    })
    // 失败：扣血
    .RegisterReducer<RoundLostAction>((state, _) => state with
    {
        Hp = state.Hp - 1
    });
```

### 第 3 步：订阅（谁关心谁登记）

```csharp
// 订阅并立即回放当前值（适合初始化 UI）
store.SubscribeWithInitValue(s => scoreLabel.Text = s.Score.ToString());
store.SubscribeWithInitValue(s => levelLabel.Text = s.Level.ToString());

// 只在变化时通知（适合副作用，如弹窗）
store.Subscribe(s => { if (s.IsCleared) ShowVictoryPanel(); });
```

### 第 4 步：游戏逻辑只负责提交事件单

```csharp
store.Dispatch(new RoundWonAction(Bonus: 100));   // 过关只要这一行
store.Dispatch(new RoundLostAction());            // 失败也只要这一行
```

**对比第 1 节的一坨代码**：全没了，只剩一句 `Dispatch`。

---

## 5. 一行 Dispatch 背后发生了什么

```
store.Dispatch(new RoundWonAction(100))
    │
    ├─ ① 按 action 类型找出所有注册过的 reducer
    ├─ ② 依次执行：newState = Reduce(oldState, action)
    ├─ ③ 比较新旧状态（默认用相等比较）：
    │      真变了 → 保存新状态，通知订阅者
    │      没变化 → 什么都不做（省性能）
    └─ ④ 通知订阅者（把【整块新状态】发给他们）
           scoreLabel 读 s.Score  → 刷新
           levelLabel 读 s.Level  → 刷新
           通关判定   读 s.IsCleared → 决定弹不弹面板
```

注意第 ④ 步：**通知时整块状态都发出去**。这带来一个性能小问题——分数变了，`levelLabel` 也被叫醒，结果发现 Level 没变，白忙一场。

**这就是第一个进阶件 Selector 要解决的。**

---

## 6. 进阶一：Selector（选择器）——只订阅我关心的那部分

### 痛点

20 个订阅者时，每次任何字段变化，20 个全部惊醒，各自读了发现自己没变。浪费。

### 解法：订阅"局部投影"

```csharp
using GFramework.Core.Extensions;   // Select / ToBindableProperty 扩展方法在这

// 只订阅 Score 这一块
store.Select(s => s.Score)
     .RegisterWithInitValue(v => scoreLabel.Text = v.ToString());

// 只订阅 Hp 这一块
store.Select(s => s.Hp)
     .RegisterWithInitValue(v => hpLabel.Text = v.ToString());
```

效果：
- 过关（Score 变、Hp 没变）→ **只有 scoreLabel 醒**
- 失败（Hp 变、Score 没变）→ 只有 hpLabel 醒

### 它怎么判断"这块变没变"？

用**旧投影值 vs 新投影值**比较（默认相等比较器，可自定义）。Store 状态变了，但你的片段没变 → 不通知。

### 派生投影（存档里没有、算出来的值）

```csharp
// 血量百分比：存档只存 Hp/MaxHp，百分比是算出来的
store.Select(s => (float)s.Hp / s.MaxHp)
     .Register(pct => healthBar.Value = pct);
```

### 缓存复用（重要）

高频访问的属性**不要每次 new**，用带 key 的缓存版：

```csharp
// Model 内暴露给 UI 的只读视图（同一 key 复用同一实例）
public IReadonlyBindableProperty<int> Score =>
    Store.GetOrCreateBindableProperty("score", s => s.Score);

public IReadonlyBindableProperty<float> HpPercent =>
    Store.GetOrCreateBindableProperty("hp_percent", s => (float)s.Hp / s.MaxHp);
```

### 桥接已有的 BindableProperty 代码

如果现有 UI 代码已经依赖 `IReadonlyBindableProperty<T>`，可以无缝换源：

```csharp
IReadonlyBindableProperty<int> health = store.ToBindableProperty(s => s.Hp);
// 之后 UI 用法完全不变：health.RegisterWithInitValue(...)
```

### 内部小知识（知道即可）

`StoreSelection` 是**懒附加**的：第一个监听器注册时才连 Store，最后一个注销时自动断开——闲置的 selector 不会一直挂着引用。

> **一句话**：`Selector = s => 我关心的字段`，把"整树订阅"变成"片段订阅"，省掉无用刷新。

---

## 7. 进阶二：Middleware（中间件）——在提交和算账之间插一层

### 痛点

想给所有状态操作**加日志 / 计时 / 调试记录**。按老办法得改每个 reducer 加打印——污染规则代码，还容易漏。

### 解法：中间件

```csharp
using GFramework.Core.Abstractions.StateManagement;

public sealed class LoggingMiddleware : IStoreMiddleware<RunState>
{
    public void Invoke(StoreDispatchContext<RunState> ctx, Action next)
    {
        // ① 进入：还没算账，能看到"收到了什么"
        Console.WriteLine($"[Store] 提交: {ctx.ActionType.Name}");

        next();   // ② 放行 —— 这行之后才真正跑 reducer

        // ③ 返回：账已算完，能看到变化前后
        Console.WriteLine($"[Store] 状态变化: {ctx.HasStateChanged}");
        Console.WriteLine($"[Store] 关卡: {ctx.PreviousState.Level} → {ctx.NextState.Level}");
    }
}

store.UseMiddleware(new LoggingMiddleware());
```

### 为什么叫"洋葱模型"？

```
LoggingMiddleware 进入
  PerformanceMiddleware 进入
    ▶ 真正执行 reducer
  PerformanceMiddleware 退出
LoggingMiddleware 退出
```

多个中间件按注册顺序**层层包裹**。`next()` 之前是"进去"，之后是"出来"。

### 能干什么

| 用途 | 说明 |
|---|---|
| 日志 / 审计 | 记录每次操作（上面示例） |
| 性能计时 | `next()` 前后打时间戳 |
| 调试探针 | 特定 action 时断点/落盘 |
| 拦截 | 某些条件下不调 `next()`（阻止本次归约） |

**不用改 reducer 一行代码。**

### 运行时临时挂载（可选）

中间件只想在某段生命周期生效（调试探针、临时规则）时，用句柄式注册，随时拆：

```csharp
var handle = store.RegisterMiddleware(new LoggingMiddleware());
// ...
handle.UnRegister();   // 拆除（进行中的 dispatch 仍用旧快照）
```

---

## 8. 进阶三：History（历史）——撤销 / 重做 / 时间旅行

### 痛点

玩家想悔棋；或者你想做"时间旅行调试"，看状态一步步怎么变的。

### 解法：构造时开启历史缓冲区

```csharp
var store = new Store<RunState>(
    new RunState(...),
    historyCapacity: 50);   // 最多记 50 步（0 = 不记录，默认值）
```

白送这些能力：

```csharp
store.Dispatch(new RoundWonAction(100));   // 第 1 步
store.Dispatch(new RoundWonAction(100));   // 第 2 步

store.Undo();              // 撤销 → 回到第 1 步后
store.Redo();              // 重做 → 又回到第 2 步后
store.TimeTravelTo(0);     // 直接跳到最开始
store.ClearHistory();      // 以当前状态为新起点，清空历史

store.CanUndo;             // true/false → 控制"撤销"按钮的置灰
store.CanRedo;
```

### 原理很朴素

每次状态变化，就把新状态追加进一个列表，并用游标记录"现在在第几页"。

`Undo` = 游标往回挪一格、读出那一页的状态；`Redo` = 往前挪。

因为状态是**不可变**的，旧页从没被涂改过，所以随时能原样取回。

### 诊断（排查用）

```csharp
store.HistoryCapacity;   // 容量
store.HistoryCount;      // 当前记录了几页
store.HistoryIndex;      // 游标位置
store.GetHistoryEntriesSnapshot();   // 每页的 state + action + 时间
```

---

## 9. 批处理：一次通知，多次修改

### 痛点

一个操作里连续改多次（比如结算：先加分、再升级、再判定通关），如果每次都通知 UI，会**重刷好几遍**。

### 解法：RunInBatch

```csharp
store.RunInBatch(() =>
{
    store.Dispatch(new AddScoreAction(100));
    store.Dispatch(new LevelUpAction());
    store.Dispatch(new CheckClearAction());
});
// ↑ 中间 3 次 dispatch 都提交了状态，但订阅者只在【最外层结束时】收到 1 次最终状态
```

| 行为 | 说明 |
|---|---|
| 批内 dispatch | 状态照常逐个更新 |
| 通知 | **折叠为一次**，只发最终状态 |
| 可嵌套 | 内层批结束不会通知，等最外层 |

---

## 10. 与 GFramework 集成：官方推荐模式

**这是最重要的一节。** Store 不是独立玩具，它有标准的落地位置——**放在 Model 里**，与 CQRS 完全兼容。

```csharp
using GFramework.Core.Model;
using GFramework.Core.StateManagement;

public class RunModel : AbstractModel
{
    // ① Store 作为 Model 的内部状态容器
    public Store<RunState> Store { get; } = new(new RunState(0, 3, 1, false), historyCapacity: 50);

    // ② 对外暴露带缓存的只读视图（UI 只消费这些）
    public IReadonlyBindableProperty<int> Score =>
        Store.GetOrCreateBindableProperty("score", s => s.Score);

    public IReadonlyBindableProperty<int> Hp =>
        Store.GetOrCreateBindableProperty("hp", s => s.Hp);

    // ③ 注册规则
    protected override void OnInit()
    {
        Store
            .RegisterReducer<RoundWonAction>((s, a) => s with
            {
                Score = s.Score + a.Bonus,
                Level = s.Level + 1,
                IsCleared = s.Level + 1 >= 5
            })
            .RegisterReducer<RoundLostAction>((s, _) => s with { Hp = s.Hp - 1 });
    }
}
```

注册进架构（模板 `ModelModule` 的风格）：

```csharp
architecture.RegisterModel(new RunModel());
```

### Command 驱动 action（分层不变）

```csharp
using GFramework.Core.Command;

public sealed class WinRoundCommand(int bonus) : AbstractCommand
{
    protected override void OnExecute()
    {
        // 还是"Command 拿 Model"的老姿势，Store 只是 Model 内部的实现细节
        this.GetModel<RunModel>()!.Store.Dispatch(new RoundWonAction(bonus));
    }
}
```

### 页面/Controller 消费只读视图

```csharp
[Log]
[ContextAware]
public partial class HudPage : Control
{
    private readonly IUnRegisterList _unRegisterList = new UnRegisterList();

    public override void _Ready()
    {
        var model = this.GetModel<RunModel>()!;

        model.Score
            .RegisterWithInitValue(v => _scoreLabel.Text = v.ToString())
            .AddToUnregisterList(_unRegisterList);

        model.Hp
            .RegisterWithInitValue(v => _hpLabel.Text = v.ToString())
            .AddToUnregisterList(_unRegisterList);
    }
}
```

> `UnRegisterList` + `AddToUnregisterList` 保证节点退出时自动注销，不泄漏。

**完整分层**：

```
[UI 页面]  点击 → SendCommand(WinRoundCommand)
     │
[Command]  取 Model → Store.Dispatch(action)
     │
[Model]    持有 Store + 注册 reducer（状态规则都在这）
     │
[Store]    算新状态 → 通知 selector
     │
[UI 页面]  selector 回调 → 刷新控件
```

Controller → Command → Model 的分层**完全没变**，只是 Model 内部状态管理升级了。

---

## 11. Store ↔ EventBus 桥接（迁移过渡用）

如果旧模块里还有一堆逻辑依赖 CQRS 事件总线，可以临时把 Store 的变化桥接过去：

```csharp
using GFramework.Core.Extensions;

var bridge = store.BridgeToEventBus(this.GetArchitecture().EventBus);
// 之后会发两类事件：
//   StoreDispatchedEvent<TState>      —— 每次 dispatch 都发（即使状态没变）
//   StoreStateChangedEvent<TState>    —— 只在状态真变化时发；批处理只发最终态

bridge.UnRegister();   // 不需要时拆除
```

> 官方建议：**桥接只作迁移过渡层**，新模块应直接依赖 Store。

---

## 12. 诊断接口（IStoreDiagnostics）

排查"状态怎么不对了"时的利器。

> ⚠️ 诊断成员定义在 **`IStoreDiagnostics<TState>`** 接口上，`Store<TState>` 类实现了它。
> 当你持有 `Store<TState>` 时可直接访问；若持有的是 `IStore<TState>`（如 `StoreBuilder.Build()` 的返回值），需要先转型。

```csharp
// 情况一：变量是 Store<TState> 类型 → 直接访问
store.SubscriberCount;        // 当前订阅者数量（查泄漏）
store.LastActionType;         // 最近一次动作类型
store.LastStateChangedAt;     // 最近一次状态变化时间
store.LastDispatchRecord;     // 最近一次分发记录（前后状态）
store.IsBatching;             // 是否正在批处理中
store.ActionMatchingMode;     // 当前匹配模式

// 情况二：变量是 IStore<TState> 类型 → 先转型
var diagnostics = (IStoreDiagnostics<RunState>)store;
diagnostics.HistoryCapacity;  // 历史容量
diagnostics.HistoryCount;     // 已记录页数
diagnostics.HistoryIndex;     // 当前游标
diagnostics.GetHistoryEntriesSnapshot();  // 每页的 state + action + 时间
```

---

## 13. 什么时候用 / 不用（决策表）

| 你的情况 | 选择 |
|---|---|
| 一个开关按钮、一条文本显示 | ❌ 别用 Store，`BindableProperty<T>` 足够 |
| 一个操作要同时改多个字段（过关=分数+关卡+标记） | ✅ Store |
| 多个界面共享同一份状态（战斗界面 + HUD 都看血量） | ✅ Store |
| 想撤销 / 重做 / 时间旅行 | ✅ Store + `historyCapacity` |
| 想统一状态规则 + 要日志排查 | ✅ Store + `middleware` |
| 状态很小、无共享、只要值变化通知 | ❌ 继续 `BindableProperty<T>` |

和另外三个机制的关系：

| 机制 | 定位 |
|---|---|
| `BindableProperty<T>` | 字段级响应式值 |
| `Store<TState>` | 聚合状态容器（本文） |
| `StateMachine` | 流程状态切换 |
| CQRS Command / Event | 跨组件通信与行为编排 |

---

## 14. 最佳实践

1. **TState 用不可变 `record`**——这是撤销/追溯/测试的基础
2. **reducer 保持纯函数**——不碰 IO、不调外部服务、不产生副作用
3. **selector 暴露局部状态**——UI 不要自己去解析整棵状态树
4. **高频访问用 `GetOrCreateBindableProperty(key, ...)`**——复用实例，避免每次 new
5. **日志/诊断走 middleware**——不要塞进 reducer
6. **默认精确类型匹配**——只有确有继承层次复用需求时才开 `IncludeAssignableTypes`
7. **引入顺序**：先把聚合状态收进某个 Model → 再把修改入口迁到 Command → 最后 Controller 用 selector 绑定

---

## 15. 常见坑

| 坑 | 说明 |
|---|---|
| **在 reducer 里做副作用** | reducer 可能被多次调用（重放/时间旅行），副作用会重复触发。副作用放订阅回调或 Command 里 |
| **直接改 state 字段** | `record` 的属性是不可变的，改不动；要用 `with` 造新对象 |
| **忘了比较器** | 默认相等比较对复杂类型可能"看着变了其实没变"，需要时传自定义 `IEqualityComparer<TState>` |
| **dispatch 中重入 dispatch** | 同一次 Dispatch 里再 Dispatch 会抛 `InvalidOperationException`；需要多次修改用 `RunInBatch` |
| **dispatch 中注销 reducer** | 当前这次 dispatch 仍用开始时的快照，注销只影响后续（不是 bug，是设计） |
| **selector 没缓存** | 在属性 getter 里 `store.Select(...)` 每次返回新实例，可能造成重复订阅；高频用 `GetOrCreateBindableProperty` |
| **历史容量开太大** | 每个历史点存一份完整状态快照，容量大且状态大时内存上涨 |

---

## 16. API 速查

### 构造

```csharp
new Store<TState>(initialState)                                  // 最简
new Store<TState>(initialState, comparer)                        // 自定义状态比较器
new Store<TState>(initialState, comparer, historyCapacity: 50)   // 开启历史
new Store<TState>(initialState, comparer, 50, StoreActionMatchingMode.IncludeAssignableTypes)

Store<TState>.CreateBuilder()
    .WithComparer(comparer)
    .WithHistoryCapacity(50)
    .WithActionMatching(StoreActionMatchingMode.IncludeAssignableTypes)
    .AddReducer<TAction>((s, a) => ...)
    .UseMiddleware(mw)
    .Build(initialState);
```

### 读写

| API | 说明 |
|---|---|
| `State` | 读当前状态快照 |
| `Dispatch<TAction>(action)` | 提交动作（唯一写入入口） |
| `RunInBatch(action)` | 批处理，通知折叠为一次 |
| `Subscribe(listener)` / `SubscribeWithInitValue(listener)` | 订阅（后者的立即回放当前值） |
| `UnSubscribe(listener)` | 取消订阅 |

### 规则注册

| API | 说明 |
|---|---|
| `RegisterReducer<TAction>(lambda)` | 链式注册（初始化用） |
| `RegisterReducerHandle<TAction>(lambda)` | 返回句柄，可运行时注销 |
| `UseMiddleware(mw)` | 链式注册中间件 |
| `RegisterMiddleware(mw)` | 返回句柄，可运行时注销 |

### 选择视图

| API | 说明 |
|---|---|
| `store.Select(s => ...)` | 投影局部状态 → `StoreSelection` |
| `store.ToBindableProperty(s => ...)` | 投影并桥接为 `IReadonlyBindableProperty<T>` |
| `store.GetOrCreateSelection(key, s => ...)` | 带缓存的选择视图 |
| `store.GetOrCreateBindableProperty(key, s => ...)` | 带缓存的绑定视图（推荐） |

### 历史

| API | 说明 |
|---|---|
| `Undo()` / `Redo()` | 撤销 / 重做 |
| `TimeTravelTo(index)` | 跳到指定历史点 |
| `ClearHistory()` | 清空历史，以当前状态为锚 |
| `CanUndo` / `CanRedo` | 可用性（控制按钮置灰） |

### 相关类型

| 类型 | 用途 |
|---|---|
| `IReducer<TState, TAction>` | reducer 接口（也可用 lambda） |
| `IStateSelector<TState, TSelected>` | 选择器接口（也可用 lambda） |
| `IStoreMiddleware<TState>` | 中间件接口 |
| `StoreDispatchContext<TState>` | 中间件里拿到的上下文（Action/前后状态/是否变化） |
| `StoreDispatchRecord<TState>` | 分发记录（诊断用） |
| `StoreHistoryEntry<TState>` | 历史条目（state + action + 时间） |
| `StoreActionMatchingMode` | 精确类型 / 含可赋值类型 |

---

## 17. 框架源码阅读入口

| 文件 | 内容 |
|---|---|
| `GFramework.Core/StateManagement/Store.cs` | Store 核心实现（dispatch 管线、历史、批处理、诊断） |
| `GFramework.Core/StateManagement/StoreBuilder.cs` | 构建器 |
| `GFramework.Core/StateManagement/StoreSelection.cs` | 局部选择视图（懒附加 + 比较器防抖） |
| `GFramework.Core/Extensions/StoreExtensions.cs` | `Select` / `ToBindableProperty` 扩展方法 |
| `GFramework.Core/Extensions/StoreEventBusExtensions.cs` | `BridgeToEventBus` 桥接 |
| `GFramework.Core.Abstractions/StateManagement/` | 接口层（IStore/IReducer/IStateSelector/…） |
| `GFramework/docs/zh-CN/core/state-management.md` | 框架官方文档（含角色面板完整示例） |
| `GFramework.Core.Tests/StateManagement/` | 框架自测（行为契约参考） |

---

## 附：完整可跑示例（归档用）

```csharp
// ── 状态与动作 ────────────────────────────────
public sealed record RunState(int Score, int Hp, int Level, bool IsCleared);
public sealed record RoundWonAction(int Bonus);
public sealed record RoundLostAction();

// ── 中间件 ────────────────────────────────────
public sealed class LoggingMiddleware : IStoreMiddleware<RunState>
{
    public void Invoke(StoreDispatchContext<RunState> ctx, Action next)
    {
        Console.WriteLine($"提交: {ctx.ActionType.Name}");
        next();
        Console.WriteLine($"变化: {ctx.HasStateChanged}，关卡 {ctx.PreviousState.Level}→{ctx.NextState.Level}");
    }
}

// ── Model 承载 Store ──────────────────────────
public class RunModel : AbstractModel
{
    public Store<RunState> Store { get; } = new(new RunState(0, 3, 1, false), historyCapacity: 50);

    public IReadonlyBindableProperty<int> Score =>
        Store.GetOrCreateBindableProperty("score", s => s.Score);

    public IReadonlyBindableProperty<int> Level =>
        Store.GetOrCreateBindableProperty("level", s => s.Level);

    protected override void OnInit()
    {
        Store.UseMiddleware(new LoggingMiddleware());
        Store.RegisterReducer<RoundWonAction>((s, a) => s with
        {
            Score = s.Score + a.Bonus,
            Level = s.Level + 1,
            IsCleared = s.Level + 1 >= 5
        });
        Store.RegisterReducer<RoundLostAction>((s, _) => s with { Hp = s.Hp - 1 });
    }
}

// ── Command ───────────────────────────────────
public sealed class WinRoundCommand(int bonus) : AbstractCommand
{
    protected override void OnExecute()
    {
        this.GetModel<RunModel>()!.Store.Dispatch(new RoundWonAction(bonus));
    }
}

// ── UI 页面订阅 ───────────────────────────────
// model.Score.RegisterWithInitValue(v => label.Text = v.ToString()).AddToUnregisterList(list);
// this.SendCommand(new WinRoundCommand(100));
// model.Store.Undo();   // 悔棋
```

---

## 附：本文示例的代码验证

本文所有核心 API 用法都有测试守护：`tests/My-GFramework-Godot-Template.Tests/StoreDocVerificationTests.cs`。

| 用例 | 验证内容 |
|---|---|
| 基本_Dispatch与Subscribe | 提交动作 + 订阅回放 |
| Selector_只通知变化片段 | 局部订阅的防抖语义 |
| ToBindableProperty_桥接可用 | 桥接 `IReadonlyBindableProperty` |
| GetOrCreateBindableProperty_同Key复用实例 | 缓存复用（`Assert.Same`） |
| Middleware_洋葱包裹并可见前后状态 | 中间件上下文 |
| History_撤销重做时间旅行 | Undo/Redo/TimeTravelTo |
| RunInBatch_通知折叠为一次 | 批处理语义 |
| 诊断_可读取订阅数与最后动作 | 诊断接口 |
| 运行时句柄_可注销中间件 | `RegisterMiddleware` + `UnRegister` |
| StoreBuilder_链式构建 | Builder 链式 API |
| 精确类型匹配_默认不响应继承层次 | `ExactTypeOnly` 默认值 |

**框架升级后跑 `dotnet test` 即可发现本文档是否过期。**
