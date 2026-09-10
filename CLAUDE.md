# CLAUDE.md

本文档为 Claude Code (claude.ai/code) 在基于本框架模板构建的项目中工作时提供指导。

项目编码规范详见 [CONVENTIONS.md](CONVENTIONS.md)，涵盖命名空间、文件结构、CQRS 约定、XML 注释标准、修饰符规范等。以下为关键要点速查与架构概览。

## 构建与测试

```bash
# 构建项目（需要 Godot .NET SDK 4.7）
dotnet build

# 运行全部测试
dotnet test

# 运行时端到端自检（Godot headless：验证架构装配 + UI→命令→事件链路）
godot --headless --path . --quit-after 300
```

测试使用 xUnit，测试项目位于 `tests/` 目录下，包含：
- 各框架能力的教程配套验证测试（Store/Config/协程/本地化/暂停/日志/UI 等）
- 仓库一致性自检（`RepositoryConsistencyTests`）：命令/事件 `sealed` 规范、
  `[ContextAware]`+`[Log]` 成对、schema↔config 数据目录、多语言 key 一致性
- 会话文件日志落在 `user://logs/`（JSON 行），排错时优先查看（游戏内 F12 打开目录）

## 关键约束速查

以下为 CONVENTIONS.md 的核心规则，开发时务必遵守：

- **命名空间** 与目录一对一映射，使用文件范围声明 `namespace X.Y.Z;`（无大括号）；事件的 C# 关键字 `event` 在命名空间中转义为 `@event`
- **事件** 全部 `public sealed class`；属性全部 `{ get; init; }` + `required`；无数据事件用分号声明 `public sealed class FooEvent;`
- **命令** 全部 `public sealed class`，统一继承 `AbstractAsyncCommand(Async)`（Godot 主线程禁止同步 CQRS）；命令输入全部 `sealed class : ICommandInput`（**禁止 struct**）
- **CQRS 调用**：节点内用 `await this.SendCommandAsync(new XxxCommand(...))`；禁止 `this.SendCommand(...)`（框架 `GuardSyncCqrs` 会抛异常）；`this.SendEvent(...)` 不受限
- **Godot 节点** 全部 `public partial class`（不 sealed）；标注 `[Log]` + `[ContextAware]`（成对，`[Log]` 在前）
- **语法糖注入必须显式绑定**：`_Ready()` 开头调用 `__InjectContextBindings_Generated()`（[GetSystem]/[GetModel]/[GetUtility]）与 `__InjectGetNodes_Generated()`（[GetNode]），漏掉则字段为 null
- **`_Ready()` 顺序**：注入 → 信号绑定 → `RegisterEvents()`（事件订阅）→ `_ = ReadyAsync()`（异步初始化）；订阅先于异步，避免初始化期间事件丢失
- **`ReadyAsync()` 中禁止 `ConfigureAwait(false)`**（续体切线程池后无法访问 Godot 节点）
- **节点引用** 在 `.Dependencies.cs` 中用 `[GetNode]` 字段注入（按字段名推断 `%唯一名`）
- **UI 页面** 不需要提取 `I*` 接口（由 UiRouter 管理，不被其他组件消费）
- **XML 注释** 中文；接口/公开方法/事件/命令必须有 `<summary>`；私有方法按需
- **提交** 格式 `<type>(<scope>): <中文描述>`，每次提交为逻辑独立的原子操作

## 提交规则

- 每次提交必须是逻辑上独立的原子操作。
- **遇到复杂变更时必须分析**：如果一次对话的修改混杂了不同功能、bug 修复或优化，必须主动分析其原子性。
- **按组件或职责拆分**：例如，对 API 格式的调整与对 UI 样式的修改应分开提交。
- **主动建议**：分析后，生成一个包含多个提交的"群组提案"，而不是把所有东西一股脑儿塞进一个提交。

## 架构

**技术栈：** Godot 4.7 + C# (.NET 10) + GFramework (0.7.1) — NuGet 上的 CQRS/ECS 框架。

**DI 引导：** `global/GameEntryPoint`（自动加载单例）创建 `GameArchitecture`，安装 5 个模块：

| 模块 | 职责 |
|---|---|
| `ModelModule` | 注册设置模型及其应用器（音频/图形/本地化） |
| `SystemModule` | 注册 UiRouter、SceneRouter、SettingsSystem、LocalizationManager |
| `UtilityModule` | 注册工具：UI/场景/纹理注册表、存储、JSON 序列化、设置仓储 |
| `ConfigModule` | 安装 GFramework Config 系统（YAML + Schema + 源生成器），注册 `ConfigRegistry`/`ConfigRuntime` |
| `StateModule` | 注册 `GameStateMachineSystem` 及状态 |

**状态 → UI 映射：** 每个状态实现 `ContextAwareStateBase`，在 `OnEnter` 中清除之前的 UI/场景，并通过 `UiRouter.Push()` 推入对应的 UI 页面。参见 `AppState`（进入时推送 `MainMenu`）。

**UI 页面** 继承 `Control`，实现 `IUiPageBehaviorProvider` + `ISimpleUiPage`。采用 partial class 模式：

| Partial 文件 | 用途 |
|---|---|
| `*.cs` | 核心：`_Ready()`（注入 → 信号 → 事件 → 异步） |
| `*.Dependencies.cs` | `[GetNode]`/`[GetSystem]` 注入字段、`ReadyAsync()` |
| `*.Properties.cs` | 字段、属性、`UiKeyStr` |
| `*.Events.cs` | `RegisterEvents()` 订阅 CQRS 事件 |
| `*.Signals.cs` | `[BindNodeSignal]` 信号 → 命令/事件桥接 |

**Entity partial 类** 遵循相同模式：`Entity.cs`、`Entity.Dependencies.cs`、`Entity.Properties.cs`、`Entity.Events.cs`、`Entity.Signals.cs`。

## 核心模式

### CQRS 通信
组件通过 GFramework 事件通信，而非 Godot 信号：
- **发送命令：** `await this.SendCommandAsync(new SomeCommand(...))`（异步命令；同步 `SendCommand` 在 Godot 主线程被框架禁止）
- **发送事件：** `this.SendEvent(new SomeEvent { ... })`
- **订阅：** 在 `RegisterEvents()` 内使用 `this.RegisterEvent<SomeEvent>(e => { ... })`，并以 `.UnRegisterWhenNodeExitTree(this)` 链式调用确保节点退出时自动注销
- 事件位于 `scripts/cqrs/<domain>/event/`，命名空间 `...cqrs.<domain>.@event`
- 命令位于 `scripts/cqrs/<domain>/command/`，命令输入位于 `scripts/cqrs/<domain>/command/input/`

### 日志与上下文
- `[Log]` 特性通过 GFramework 源代码生成器自动生成静态 `Log` 属性
- `[ContextAware]` 特性自动注入 GFramework 架构上下文
- 两者**成对使用**，`[Log]` 在前
- 日志双路输出：Godot 控制台 + 会话文件（`user://logs/YYYYMMDD_HHmmss.log`，JSON 行；F12 打开日志目录）

### 数据驱动配置（Config 系统）
- `schemas/*.schema.json` 描述结构 → 源生成器产出强类型类（命名空间 `GFramework.Game.Config.Generated`）
- `config/<域>/*.yaml` 提供数据；`ConfigModule` 自动注册全部生成的表
- 业务读取入口：`this.GetUtility<ConfigRuntime>()`
- 已知边界：schema 数组字段暂不可用（生成 `IReadOnlyList` 无法被 YamlDotNet 反序列化）

### 本地化
- 语言表位于 `localization/{语言码}/{表名}.json`，由 `LocalizationManager` 载入（编辑器直读 `res://`，导出同步 `user://`）
- 取词：`this.GetSystem<ILocalizationManager>().GetText("common", "key")`

## 框架模块清单

| 模块 | 文件 | 注册内容 |
|---|---|---|
| ModelModule | `scripts/module/ModelModule.cs` | `SettingsModel` + Audio/Graphics/Localization 应用器 |
| SystemModule | `scripts/module/SystemModule.cs` | `UiRouter`、`SceneRouter`、`SettingsSystem`、`LocalizationManager` |
| UtilityModule | `scripts/module/UtilityModule.cs` | UI/场景/纹理注册表、存储、JSON 序列化、设置仓储 |
| ConfigModule | `scripts/module/ConfigModule.cs` | `ConfigRegistry`、`ConfigRuntime`（Config 系统接入） |
| StateModule | `scripts/module/StateModule.cs` | `GameStateMachineSystem` + `AppState` |

## 示例 CQRS 域

框架保留了以下通用 CQRS 域作为参考：

- `scripts/cqrs/audio/command/` — 音量控制命令（Master/Bgm/Sfx）
- `scripts/cqrs/graphics/` — 分辨率和全屏切换命令
- `scripts/cqrs/setting/` — 设置保存/重置/查询命令
- `scripts/cqrs/game/` — 游戏流程命令（`StartGameCommand` → `GameStartedEvent` 演示命令-事件-UI 闭环、`ExitGameCommand`）

**示例页面**：`scripts/ui/menu/MainMenu.cs`（语法糖 + CQRS 端到端链路的完整参考）。

## 目录结构约定

```
scripts/
├── component/       # 通用组件（HFSM、行为树、TweenTree…）
├── constants/       # 全局常量
├── core/            # 框架核心接线：架构引导 GameArchitecture、environment/、input/、
│                    #   resource/（*Config 资源类型）、scene/、state/、ui/（UiRouter 等）
├── cqrs/            # CQRS 命令/事件/查询（按业务域划分）
├── data/            # 持久化数据（设置数据位置提供者等）
├── enums/           # 枚举（UI Key、场景 Key、纹理 Key 等）
├── framework/       # 对 GFramework 的自研扩展：config/、event/、localization/、
│                    #   logging/（会话日志）、registry/（资源注册表）
├── module/          # DI 模块
├── ui/              # UI 页面（按域分子目录：menu/、hud/…）
└── utility/         # 与框架无关的纯工具（GameUtil 等）

scenes/              # 场景（ui/menu/… + main.tscn）
config/              # YAML 配置数据（一对象一文件）
schemas/             # JSON Schema（源生成器自动拾取）
localization/        # 语言表（{语言码}/{表名}.json）
```

> 业务代码按业务域在 `scripts/` 下自建子目录（参考 `scripts/ui/menu/`），不要为"未来可能的实体/模型/系统"预留空目录。

## 深入学习

框架能力的详细用法见 `docs/`：

- `docs/guides/` — 教学文档（对象池/ECS/Store/Config/协程/本地化/暂停/音频/日志/UI 弹窗），每篇附验证测试
- `docs/research/gframework-extensions/` — 框架扩展接口研究（输入域/暂停栈/Store/服务层）
- `docs/plan/` — 组件规划与设计文档
