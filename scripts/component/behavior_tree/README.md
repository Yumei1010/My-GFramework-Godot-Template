# 行为树（Behavior Tree）

`scripts/component/behavior_tree/` 下的行为树组件，用于 AI 决策。与 HFSM 互补：**HFSM 管"状态切换"，行为树管"做什么"**。

## 架构

所有节点都是 **Godot 节点**（继承 `Node`），在场景树中拖拽拼装即组成行为树：

```
IBehaviorNode（接口契约：Execute()）
    └── BehaviorNode（抽象基类，继承 Node，提供 ChildNodes）
        ├── ActionNode（叶子：动作）
        ├── ConditionNode（叶子：条件）
        ├── SequenceNode（复合：顺序）
        ├── SelectorNode（复合：选择）
        └── BehaviorTree（根：自动每帧 Tick）
```

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

## 快速上手：敌人 AI（场景树拼装）

在场景树中搭出如下层级（或代码 `AddChild` 动态组装）：

```
BehaviorTree（根，自动每帧驱动）
└── SelectorNode（选一个策略）
    ├── SequenceNode（策略1：有目标 → 攻击）
    │   ├── ConditionNode：有目标？
    │   └── ActionNode：攻击
    └── ActionNode：巡逻（兜底）
```

叶子节点绑定逻辑有两种方式：

**方式一：编辑器绑定 Callable** — 把 `Action` / `Condition` 属性设为任意节点的某个方法：
- `ConditionNode.Condition` → 返回 `bool` 的方法
- `ActionNode.Action` → 返回 `int`（0成功/1失败/2运行中）、`bool` 或 `void` 的方法

**方式二：代码注入委托**（更灵活）：

```csharp
GetNode<ConditionNode>("%HasTarget").SetCondition(() => hasTarget);
GetNode<ActionNode>("%Attack").SetAction(() =>
{
    if (Arrived()) return NodeStatus.Success;
    MoveToward(target);
    return NodeStatus.Running;   // 没到，下一帧继续走
});
```

## Running：多帧动作（精髓）

"走向目标"不是一帧能完成的，返回 `Running` 让下一帧**从当前节点继续**，而不是从头重跑。

## 组合示例：有弹药射击，没弹药装弹

```
AttackSequence (Sequence)
├── ConditionNode：有目标？      （闸门）
└── AmmoSelector (Selector)
    ├── SequenceNode
    │   ├── ConditionNode：有弹药？
    │   └── ActionNode：射击
    └── ActionNode：装弹         （回退）
```

## 常用查询

| 成员 | 含义 |
|---|---|
| `tree.Tick()` | 手动执行一帧（`AutoTick=false` 时用），返回根节点结果 |
| `tree.LastStatus` | 上一帧结果（Success/Failure/Running/null） |

## 注意事项

- `SequenceNode` / `SelectorNode` 内部**记住执行位置**：Running 的子节点下一帧继续，成功后从下一节点继续
- 每次整体成功/失败后，位置重置——下一帧从头开始
- 节点可任意嵌套组合（树没有深度限制）
- Selector 的子节点若为"条件+动作"，应包一层 `SequenceNode`（否则条件满足时 Selector 直接成功，动作不会执行）

## 运行演示

打开 `scenes/behavior_tree_demo.tscn` 运行，控制台会打印行为树逐帧执行日志，
完整展示：**攻击（Running 多帧）→ 弹药耗尽装弹（Selector 回退）→ 目标消失巡逻（策略切换）**。

对应演示控制器：`scripts/demo/behavior_tree/BehaviorTreeDemoController.cs`
