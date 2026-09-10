# GFramework Store（Redux 风格状态管理）深度研究

> 日期：2026-09 | 源码：`GFramework.Core.Abstractions/StateManagement/` + `GFramework.Core/StateManagement/`
> 文档：`GFramework/docs/zh-CN/core/state-management.md`（493 行，覆盖完整）
> 定位：补足 `BindableProperty<T>` 在**复杂聚合状态树**下的能力，不替代 CQRS/Model/StateMachine

## 结论（TL;DR）

`Store<TState>` = 完整 Redux 实现（1523 行）：统一 Dispatch 入口 + reducer 归约 + middleware 管线 + 可选历史(撤销/重做/时间旅行) + 批处理折叠 + 按片段订阅。**与 GFramework 其余机制并存**：

| 机制 | 定位 |
|---|---|
| `BindableProperty<T>` | 字段级响应式值 |
| `Store<TState>` | 聚合状态容器（一次操作多字段协同、多模块共享） |
| `StateMachine` | 流程状态切换 |
| CQRS Command/Event | 跨组件通信 / 行为编排（Store 是其"内部状态实现"） |

**模板现状**：未接线（无 Store 用法）。**这是 GFramework 四件套中唯一没在模板里示范的**——适合沉淀成教学文档/示例。

## 一、核心接口

```csharp
public interface IReadonlyStore<out TState>
{
    TState State { get; }
    IUnRegister Subscribe(Action<TState> listener);              // 订阅状态变化
    IUnRegister SubscribeWithInitValue(Action<TState> listener); // 订阅 + 立即回放当前值
    void UnSubscribe(Action<TState> listener);
}

public interface IStore<out TState> : IReadonlyStore<TState>
{
    bool CanUndo { get; }          // 可撤销
    bool CanRedo { get; }          // 可重做
    void Dispatch<TAction>(TAction action);       // 唯一状态写入入口
    void RunInBatch(Action batchAction);          // 批处理（通知折叠为一次）
    void Undo(); void Redo();
    void TimeTravelTo(int historyIndex);          // 时间旅行
    void ClearHistory();
}

public interface IReducer<TState, in TAction>
{
    TState Reduce(TState currentState, TAction action);   // 纯函数归约
}

public interface IStateSelector<in TState, out TSelected>
{
    TSelected Select(TState state);                       // 状态投影
}

public interface IStoreMiddleware<TState>
{
    void Invoke(StoreDispatchContext<TState> context, Action next);  // 洋葱包裹
}
```

## 二、Store 实现要点（Store.cs 1523 行）

| 机制 | 行为 |
|---|---|
| **reducer 注册** | `RegisterReducer<T>(lambda)` 链式；**同 action 多 reducer 按注册序归约**；`RegisterReducerHandle` 返回可注销句柄（dispatch 中注销只影响后续，当前用快照） |
| **Dispatch 串行门** | `_dispatchGate` 防重入；reducer/middleware 较重逻辑只持 dispatch 门，不占状态锁 |
| **状态判变** | `IEqualityComparer<TState>` 比较，**只在真正变化时通知订阅者**（可传自定义 comparer） |
| **middleware 管线** | `ExecuteDispatchPipeline` 逆序包裹 reducer（洋葱模型），中间件快照每次 dispatch 抓取 |
| **多态 action 匹配** | `StoreActionMatchingMode`：默认 `ExactTypeOnly`；`IncludeAssignableTypes` 支持基类/接口 action 复用 reducer |
| **历史缓冲区** | `historyCapacity > 0` 开启；游标 `_historyIndex` 驱动 Undo/Redo/TimeTravel；`StoreHistoryEntry` 含 state+action+时间 |
| **批处理折叠** | `RunInBatch` 内多次 dispatch 各自提交，**通知在最外层结束后折叠为一次最终回放** |
| **诊断** | `IStoreDiagnostics`：订阅数/最后 action/最后分发记录/历史游标/批处理态/`GetHistoryEntriesSnapshot()` |

## 三、StoreSelection：按片段订阅（UI 绑定关键，405 行）

把"整棵树订阅"变"局部片段订阅"，桥接 `IReadonlyBindableProperty<T>`：

```csharp
// 缓存式选择视图（推荐：Model 内高频访问用）
Store.GetOrCreateBindableProperty("health", state => state.Health);       // key + selector
Store.GetOrCreateSelection<float>("hp_pct", s => (float)s.Health / s.MaxHealth); // 派生投影

// 或扩展方法
store.Select(state => state.Health);                       // → StoreSelection
IReadonlyBindableProperty<int> p = store.ToBindableProperty(s => s.Health); // 桥接
```

**实现亮点**：
- **懒附加**：首监听器注册才连 Store，末监听器注销即释放（闲置 selection 不挂引用链）
- **比较器防抖**：selection 自身 comparer 判变，Store 变了但片段没变 → 不通知
- **RegisterWithInitValue 不丢更新**：初始化回放与后续变化之间的竞态用 PendingValue 机制兜底

## 四、GFramework 官方模式：Model 承载 Store

**这是与 CQRS 打通的标准姿势**（三层各司其职）：

```csharp
// 1. 状态 + action（record）
public sealed record PlayerPanelState(string Name, int Health, int MaxHealth, int Level);
public sealed record DamagePlayerAction(int Amount);

// 2. Model 承载 Store，selector 暴露只读视图
public class PlayerPanelModel : AbstractModel
{
    public Store<PlayerPanelState> Store { get; } = new(new(...));
    public IReadonlyBindableProperty<int> Health =>
        Store.GetOrCreateBindableProperty("health", s => s.Health);   // 带缓存
    protected override void OnInit()
    {
        Store.RegisterReducer<DamagePlayerAction>((s, a) => s with { Health = ... });
    }
}

// 3. Command 驱动 action（GFramework 分层不变）
public sealed class DamagePlayerCommand(int amount) : AbstractCommand
{
    protected override void OnExecute()
    {
        this.GetModel<PlayerPanelModel>().Store.Dispatch(new DamagePlayerAction(amount));
    }
}

// 4. Controller 消费只读视图
model.Health.RegisterWithInitValue(h => ...).AddToUnregisterList(_unRegisterList);
```

**Command 仍走 CQRS 总线**——Store 只是 Model 内部状态实现，不改变"Controller → Command → Model"的分层。

## 五、Store ↔ EventBus 桥接（迁移过渡）

```csharp
var bridge = store.BridgeToEventBus(eventBus);
// 发 StoreDispatchedEvent<TState>：每次 dispatch（即使状态没变）
// 发 StoreStateChangedEvent<TState>：只在状态变化时；批处理只发最终态
bridge.UnRegister();  // 拆除
```

**文档明确**：桥接只作迁移过渡层，新模块应直接依赖 Store。

## 六、什么时候用 / 不用

**用**（BindableProperty 撑不住时）：
- 一次操作协同更新多个字段
- 同一业务操作在多个界面复用
- 想集中"状态结构 + 变化规则"
- 要 middleware / 调试记录 / 撤销重做 / 时间旅行

**不用**（继续 BindableProperty）：
- 单一字段绑 UI、状态小、无跨模块共享、只需值变化通知

## 七、最佳实践（文档官方）

1. `TState` 用不可变 record；reducer 纯函数、无副作用
2. selector 暴露局部状态，UI 不自己解析整树
3. 日志/诊断走 middleware，不塞进 reducer
4. 默认精确类型匹配，确有继承复用再开多态
5. 引入顺序：先聚合状态封装进 Model → 修改入口迁 Command → Controller 用 selector 绑定

## 八、与模板落地场景的关系

**候选场景**：
- **结算/计分聚合**：一次结算多字段协同（分数/解锁/统计）→ Store + batch
- **会话状态**（一轮游戏的进度/词条/生命）→ Model 承载 Store 比散 BindableProperty 干净
- **撤销需求**：若玩法要支持"悔棋/回退"→ historyCapacity

**模板沉淀候选**：Store 是四件套唯一未示范的——可加教学文档 `docs/guides/store.md` + 一个小 Model 承载示例（沿用角色面板样例），让模板覆盖"聚合状态"档位。

## 相关

- 扩展总览：`docs/research/gframework-extensions/README.md`
- Store 配套文档在框架 `docs/zh-CN/core/`：`state-management.md` / `property.md` / `model.md`
