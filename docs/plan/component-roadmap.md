# 模板组件规划

分支：main
更新：2026-09-03

## 现状（已覆盖）

| 能力 | 组件 |
|---|---|
| 状态切换 | `hierarchical_state_machine`（HFSM + 子状态机） |
| 行为决策 | `behavior_tree`（Godot 节点版可视化） |
| 动作排队 | `action_queue`（串行异步步骤） |
| 动画编排 | `tween_tree`（Tween 节点树，SubtweenTweener 组合） |
| 架构通信 | CQRS（命令/事件/查询）+ 频段事件总线（ChannelEventBus） |
| 场景/UI 管理 | SceneRouter / UiRouter / UiFactory |
| ECS | Arch（UseArch 接入） |

## 规划方向

### 1. 框架现成但模板未接入（教用法 > 造轮子）

| 能力 | GFramework 位置 | 接入价值 |
|---|---|---|
| **暂停系统** | `GFramework.Godot/Pause` | 暂停栈/分组暂停，几乎必备 |
| **对象池** | `GFramework.Godot/Pool` | 高频生成复用（子弹/特效） |
| **协程** | `GFramework.Godot/Coroutine` | 已用但无示例封装 |

**研究结论（对象池）**：GFramework `AbstractObjectPoolSystem<TKey,TObject>` 已完整实现
（Acquire/Release/Prewarm/统计/防双重释放 + `OnAcquire/OnRelease/OnPoolDestroy` 生命周期），
`AbstractNodePoolSystem` 支持 PackedScene 实例化。
→ **不重复造轮子**，补：PackedScene 快捷接入层 + 使用示例（详见对象池设计）。

### 2. 常用独立组件（按优先级）

| 优先级 | 组件 | 说明 |
|---|---|---|
| ⭐⭐⭐ | 音频管理器 | 封装 AudioStreamPlayer，SFX/BGM/音量，CQRS 化 |
| ⭐⭐⭐ | 计时器/倒计时 | 限时/冷却/QTE，节点 or 纯逻辑 |
| ⭐⭐ | 通用 UI 弹窗 | 确认/Toast，配合 UiRouter Modal |
| ⭐⭐ | 资源加载器 | 异步加载场景/纹理 |
| ⭐ | 存档/进度 | 游戏进度（区别于设置） |
| ⭐ | 调试面板 | FPS/参数热调 |

### 3. 游戏类型相关（按需，不入模板核心）

卡牌/回合、战斗/动作（输入缓冲/连击）、解谜/叙事（对话/任务）、平台（移动控制器）

## 教程文档（docs/guides/）

框架能力用**教学文档**而非示例组件承载（避免代码重复维护）：

- [x] `docs/guides/object-pool.md` — 对象池详解（框架原生 AbstractNodePoolSystem 用法）
- [x] `docs/guides/ecs.md` — Arch ECS 详解（接入/组件/系统/驱动/示例）
- 原 object_pool 示例代码已删除（教学由文档承担）

## 待办

- [ ] Pause 教程
- [ ] 音频管理器教程
- [ ] 计时器教程
- [ ] UI 弹窗教程
