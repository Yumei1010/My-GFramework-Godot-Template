# 行为树（Behavior Tree）

`scripts/component/behavior_tree/` 下的行为树组件，用于 AI 决策。与 HFSM 互补：**HFSM 管"状态切换"，行为树管"做什么"**。

## 核心概念

行为树是一棵树：**叶子**是动作/条件，**非叶子**是控制节点。每帧从根节点往下走一遍，每个节点返回一个结果：

| 结果 | 含义 |
|---|---|
| `Success` | 成功（动作完成 / 条件满足） |
| `Failure` | 失败（动作做不了 / 条件不满足） |
| `Running` | 执行中（多帧任务），下一帧继续 |

### 节点类型

| 节点 | 作用 | 返回规则 |
|---|---|---|
| `ActionNode` | 做一件事（攻击、移动、装弹） | 由动作决定 |
| `ConditionNode` | 判断条件（闸门） | 满足→Success，否则 Failure |
| `SequenceNode` | 顺序：**先 A 再做 B 再做 C** | 遇 Failure 整体失败；全 Success 才成功 |
| `SelectorNode` | 选择：**优先 A，不行就 B，再不行 C** | 遇 Success 整体成功；全 Failure 才失败 |

---

## 快速上手：敌人 AI

```csharp
var tree = new BehaviorTree(
    new SelectorNode(                          // 选一个策略（从上往下挑）
        new SequenceNode(                      // 策略1：有目标 → 攻击
            new ConditionNode(() => hasTarget),
            new ActionNode(Attack)),           // Attack 返回 Success/Failure/Running
        new ActionNode(Patrol)));              // 策略2：没目标 → 巡逻

tree.Start();        // 没有额外初始化（可省略）
// _Process 里每帧：
tree.Tick();
```

## Running：多帧动作（精髓）

"走向目标"不是一帧能完成的，返回 `Running` 让下一帧**从当前节点继续**，而不是从头重跑：

```csharp
var move = new ActionNode(() =>
{
    if (Arrived()) return NodeStatus.Success;
    MoveToward(target);          // 每帧走一点
    return NodeStatus.Running;   // 没到，下一帧继续走
});

var tree = new BehaviorTree(
    new SequenceNode(
        new ConditionNode(() => hasTarget),
        move,                    // 走到为止
        new ActionNode(Attack)));
```

## 组合示例：有弹药射击，没弹药装弹

```csharp
new SelectorNode(
    new SequenceNode(
        new ConditionNode(() => hasAmmo),   // 闸门
        new ActionNode(Shoot)),
    new ActionNode(Reload));                 // 回退
```

## 常用查询

| 成员 | 含义 |
|---|---|
| `tree.Tick()` | 执行一帧，返回根节点结果 |
| `tree.LastStatus` | 上一帧结果（Success/Failure/Running/null） |

## 注意事项

- `SequenceNode` / `SelectorNode` 内部**记住执行位置**：Running 的子节点下一帧继续，成功后从下一节点继续
- 每次整体成功/失败后，位置重置——下一帧从头开始
- 节点可任意嵌套组合（树没有深度限制）
- 树节点是**无状态**的（除了复合节点的执行位置），可在多个对象间共享同一棵树定义
