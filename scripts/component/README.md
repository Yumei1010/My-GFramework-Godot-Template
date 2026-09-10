# 可复用组件（scripts/component/）

本目录收录**与框架无关的自研组件**。组件分两种风格，按场景选择：

## 两种风格

| 风格 | 组件 | 构建方式 | 适用场景 |
|---|---|---|---|
| **Godot 节点版** | `behavior_tree`、`tween_tree` | 场景树拼装 + 检查器调参 | 需要可视化编排、子树复用、非程序员参与 |
| **纯逻辑版** | `hierarchical_state_machine`、`action_queue` | C# 代码构建 | 逻辑复杂、需要单元测试、无需编辑器编排 |

**选择建议**：能在编辑器里拼出来、且希望复用片段 → 节点版；逻辑分支多、需要单测覆盖 → 纯逻辑版。
（两者可混用，例如节点版行为树的 Action 节点内部用纯逻辑 HFSM 做子决策。）

## 组件清单

| 组件 | 说明 |
|---|---|
| `hierarchical_state_machine` | 分层状态机：父状态可挂子状态机，进入/退出递归到最深层 |
| `behavior_tree` | 行为树（节点版）：Sequence / Selector / Action / Condition |
| `action_queue` | 动作队列：异步步骤串行执行，支持异常传播与等待空闲 |
| `tween_tree` | Tween 动画树（节点版）：Sequence / Parallel / Property / Interval |
| `state_machine` | 状态契约 `IState`（Enter / Process / Exit），供分层状态机使用 |

## 测试

| 组件 | 覆盖 |
|---|---|
| `action_queue` | 单元测试（串行顺序、运行中入队、清空、异常传播、等待空闲、并发入队） |
| `hierarchical_state_machine` | 单元测试（状态切换、嵌套子机、转换条件） |
| `behavior_tree` / `tween_tree` | 依赖 Godot 运行时，通过 headless 运行验证（`godot --headless --path . --quit-after 300`） |

## 相关文档

- 行为树 / 状态机 / Tween 的用法与设计说明见各组件目录下的 `README.md`
- 框架原生能力（对象池、协程、暂停栈等）见 `docs/guides/`
