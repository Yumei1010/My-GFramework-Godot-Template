# 分层有限状态机（HFSM）

`scripts/component/hierarchical_state_machine/` 下的分层状态机组件，用于管理对象的状态切换。

## 三个概念，先懂再用

| 概念 | 文件 | 一句话 |
|---|---|---|
| **状态** | `IState` | 进入 / 每帧更新 / 退出，三个时机 |
| **转换** | `Transition` + `ITransitionCondition` | "从哪个状态、满足什么条件、切到哪个状态" |
| **分层** | `HierarchicalStateMachine` + `AttachSubMachine` | 状态里还能套一层状态机，父管大逻辑、子管细节 |

---

## 快速上手：角色 AI

```csharp
// 1. 定义状态（实现 IState）
public sealed class MoveState : IState
{
    public void Enter()              { /* 播放走路动画 */ }
    public void Process(double delta) { /* 每帧朝目标移动 */ }
    public void Exit()               { /* 停止走路动画 */ }
}

public sealed class FightState : IState
{
    public void Enter()              { /* 切换到战斗姿态 */ }
    public void Process(double delta) { /* 通用战斗逻辑 */ }
    public void Exit()               { /* 收起武器 */ }
}

// 2. 组装状态机
bool hasTarget = false;

var fsm = new HierarchicalStateMachine()
    .AddState(new MoveState())
    .AddState(new FightState())
    .AddTransition(new MoveState(), new FightState(), new BoolCondition(() => hasTarget))
    .AddTransition(new FightState(), new MoveState(), new BoolCondition(() => !hasTarget));

// 3. 启动 & 每帧驱动
fsm.Start();
// _Process 里：
fsm.Process(delta);
```

## 分层：战斗里再分近战/远程

```csharp
var melee = new MeleeState();
var ranged = new RangedState();
bool distanceFar = false;

// 子状态机：只在"战斗"状态内部生效
var fightSub = new HierarchicalStateMachine()
    .AddState(melee)
    .AddState(ranged)
    .AddTransition(melee, ranged, new BoolCondition(() => distanceFar));

var fsm = new HierarchicalStateMachine()
    .AddState(move)
    .AddState(fight)
    .AddTransition(move, fight, new BoolCondition(() => hasTarget))
    .AddTransition(fight, move, new BoolCondition(() => !hasTarget))
    .AttachSubMachine(fight, fightSub);   // ← 给"战斗"状态挂子机

fsm.Start();
// 进入 fight 时自动进入 melee；每帧更新递归到最深层
```

## 生命周期顺序（重要）

```
进入：  父状态 Enter → 子状态 Enter（若父状态挂了子机）
更新：  递归到最深层子状态 Process → 逐层向上检查转换
退出：  子状态 Exit → 父状态 Exit（自底向上）
```

## 常用查询

| 成员 | 含义 |
|---|---|
| `fsm.CurrentState` | 本机当前状态 |
| `fsm.ActiveMachine` | 当前活动的最深层状态机（含子机的递归） |
| `fsm.ActiveMachine.CurrentState` | 当前真正在活动的状态 |
| `fsm.SubMachine` | 当前状态挂的子状态机（无则 null） |
| `fsm.Parent` | 父状态机（顶级为 null） |

## 自定义转换条件

`ITransitionCondition` 只有一个方法，满足就返回 `true`。`BoolCondition` 是通用实现：

```csharp
public sealed class HasLowHpCondition(Func<float> getHp) : ITransitionCondition
{
    public bool ShouldTransition() => getHp() < 0.3f;
}
```

## 注意事项

- 转换按**添加顺序**检查，第一个满足条件的生效
- `AddTransition` 的 `from` / `to` 状态**不需要**事先 `AddState`（但建议加上，语义更清晰）
- 一个状态只能挂一个子状态机
- 状态机可嵌套多层，不限于两层
