# 模板组件规划

分支：main
更新：2026-09（教程系列推进后）

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
| 框架能力接入 | 设置（音频/图形/本地化）、Config 配置、会话文件日志 |

## 组件决策记录

### ✅ 已由框架提供（教学文档承载，不造轮子）

框架自带能力一律**不重复实现**，用教程 + 接入示例承载：

| 能力 | 框架位置 | 文档 |
|---|---|---|
| 对象池 | `AbstractNodePoolSystem` | `guides/object-pool.md` |
| ECS | Arch（`UseArch`） | `guides/ecs.md` |
| Store 状态管理 | `Store<TState>` | `guides/store.md` |
| Config 配置系统 | `YamlConfigLoader` + 源生成器 | `guides/config.md` |
| 协程 | `CoroutineScheduler` + 20 指令 | `guides/coroutine.md` |
| 本地化 | `LocalizationManager`（文件驱动） | `guides/localization.md` |
| 暂停栈 | `PauseStackManager` + `GodotPauseHandler` | `guides/pause.md` |
| 音频音量 | `GodotAudioSettings` + `AudioBusMap` | `guides/audio.md` |
| UI 路由/弹窗 | `UiRouter` + `UiLayer` + `InteractionProfile` | `guides/ui.md` |
| 日志 | `ILogAppender` 管线 + 会话文件组件 | `guides/logging.md` |
| 资源管理 | `ResourceManager` + `IResourceLoader` | `research/gframework-extensions/game-services.md` |
| 存档/进度 | `DataRepository` / `SaveRepository` / `ISaveMigration` | 同上 |
| 输入（三端） | `IInputBindingStore` / `IInputDeviceTracker` | `research/gframework-extensions/input-domain.md` |

> 暂停栈教程已含 5 步接入指南，但**模板尚未实际接线**（保留给具体项目按需接入）。

### ❌ 不纳入模板（定制玩法，业务项目自行实现）

模板组件的准入标准：**跨项目通用**。以下经评估属"具体玩法特性"，留在业务项目：

| 项 | 不纳入原因 |
|---|---|
| **计时器/倒计时**（限时+时间缩放+增减时间） | Twenty-four 类限时解谜的定制需求，非通用能力；如需要，业务项目基于 `ITimeProvider` / `Tick(delta)` 自实现即可 |
| ~~输入适配（手柄/键盘/触摸热切换）~~ | 框架已提供设备跟踪与语义动作地基（见上表），业务侧按需接线 |
| ~~存档/进度~~ | 框架 `SaveRepository` 已提供，教程承载 |
| ~~通用 UI 弹窗~~ | 框架 `UiRouter` + `UiLayer` 已提供，教程承载 |

### 3. 游戏类型相关（按需，不入模板核心）

卡牌/回合、战斗/动作（输入缓冲/连击）、解谜/叙事（对话/任务）、平台（移动控制器）

## 教程文档（docs/guides/，10 篇）

框架能力用**教学文档**而非示例组件承载（避免代码重复维护），每篇配验证测试防过期：

| 文档 | 内容 |
|---|---|
| `object-pool.md` | 对象池（框架原生 AbstractNodePoolSystem） |
| `ecs.md` | Arch ECS（接入/组件/系统/驱动） |
| `store.md` | Store 状态管理（Redux 风格 + Model 承载；11 测试） |
| `config.md` | Config 配置系统（YAML + Schema + 源生成器；5 测试） |
| `coroutine.md` | 协程（调度器/指令/CQRS 集成；6 测试） |
| `localization.md` | 本地化（文件驱动表/回退链/格式化器；8 测试） |
| `pause.md` | 暂停栈（分组/令牌/UI 联动/接入；9 测试） |
| `audio.md` | 音频设置与音量（CQRS 域/总线映射；4 测试） |
| `ui.md` | UI 路由与弹窗（分层/交互语义/守卫；8 测试） |
| `logging.md` | 日志（体系/双路输出/会话文件/导出；6 测试） |

> 原 object_pool 示例代码已删除（教学由文档承担）。

## 待办

| 项 | 说明 |
|---|---|
| 调试面板（可选） | FPS/参数热调——模板如需可加 |
| 暂停栈模板接线（可选） | 按 `guides/pause.md` 5 步接入（当前仅文档） |
| 深度研究补充（按需） | `research/gframework-extensions/` 已有 6 篇；其余域（网络/移动端最佳实践）按需 |
