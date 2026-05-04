# CLAUDE.md

本文档为 Claude Code (claude.ai/code) 在本仓库中工作时提供指导。

项目编码规范详见 [CONVENTIONS.md](CONVENTIONS.md)，涵盖命名空间、文件结构、CQRS 约定、XML 注释标准、修饰符规范等。以下为关键要点速查与架构概览。

## 构建与测试

```bash
# 构建项目（需要 Godot .NET SDK 4.6）
dotnet build

# 运行全部测试
dotnet test

# 运行单个测试类
dotnet test --filter "FullyQualifiedName~CalculateHelperBinaryTests"

# 运行单个测试方法
dotnet test --filter "FullyQualifiedName~CalculateHelperBinaryTests.Add_TwoIntegers"
```

测试使用 xUnit，位于 `tests/Time-To-Twenty-four.Tests/`。测试项目直接引用主项目——纯 C# 逻辑（Mode 计算类）的单元测试无需 Godot 运行时。

## 关键约束速查

以下为 CONVENTIONS.md 的核心规则，开发时务必遵守：

- **命名空间** 与目录一对一映射，使用文件范围声明 `namespace X.Y.Z;`（无大括号）；事件的 C# 关键字 `event` 在命名空间中转义为 `@event`
- **事件** 全部 `public sealed class`；属性全部 `{ get; init; }` + `required`；无数据事件用分号声明 `public sealed class FooEvent;`
- **命令** 全部 `public sealed class`，继承 `AbstractCommand(Async)`；命令输入全部 `sealed class : ICommandInput`（**禁止 struct**）
- **Godot 节点** 全部 `public partial class`（不 sealed）；标注 `[Log]` + `[ContextAware]`（成对，`[Log]` 在前）
- **节点引用** 在 `.Dependencies.cs` 中用 `GetNode<T>("%NodeName")`，使用接口类型（若存在）
- **UI 页面** 不需要提取 `I*` 接口（由 UiRouter 管理，不被其他组件消费）
- **XML 注释** 中文；接口/公开方法/事件/命令必须有 `<summary>`；私有方法按需
- **提交** 格式 `<type>(<scope>): <中文描述>`，每次提交为逻辑独立的原子操作

## 提交规则

- 每次提交必须是逻辑上独立的原子操作。
- **遇到复杂变更时必须分析**：如果一次对话的修改混杂了不同功能、bug 修复或优化，必须主动分析其原子性。
- **按组件或职责拆分**：例如，对 API 格式的调整与对 UI 样式的修改应分开提交。
- **主动建议**：分析后，生成一个包含多个提交的"群组提案"，而不是把所有东西一股脑儿塞进一个提交。

## 架构

**技术栈：** Godot 4.6 + C# (.NET 10) + GFramework (0.0.177) — NuGet 上的 CQRS/ECS 框架。

**DI 引导：** `global/GameEntryPoint`（自动加载单例）创建 `GameArchitecture`，安装 4 个模块：
- `ModelModule` — 设置模型及其应用器（音频/图形/本地化）
- `SystemModule` — UiRouter、SceneRouter、SettingsSystem
- `UtilityModule` — 注册表、存储、序列化、工厂
- `StateModule` — `GameStateMachineSystem`（含 5 个状态）

**状态 → UI 映射：** 每个状态（MainMenuState、CalculateMenuState、OptionsMenuState、CreditsState、GameOverState）在进入时清除之前的 UI/场景，并通过 `UiRouter.Push()` 推入自己的 UI 页面。

**UI 页面** 继承 `Control`，实现 `IUiPageBehaviorProvider` + `ISimpleUiPage`。采用 partial class 模式：

| Partial 文件 | 用途 |
|---|---|
| `*.cs` | 核心：`_Ready()` 调用 `ReadyAsync()` → `ConnectSignal()` → `RegisterEvent()` |
| `*.Dependencies.cs` | Godot 节点引用（`%NodeName`）、`ReadyAsync()` 初始化逻辑 |
| `*.Properties.cs` | 字段、属性、`UiKeyStr` |
| `*.Events.cs` | 通过 `RegisterEvent()` 订阅 CQRS 事件 |
| `*.Signals.cs` | Godot 信号 → CQRS 事件桥接（`ConnectSignal()`） |

**Entity partial 类** 遵循相同模式：`Entity.cs`、`Entity.Dependencies.cs`、`Entity.Properties.cs`、`Entity.Events.cs`、`Entity.Signals.cs`。

## 核心模式

### CQRS 通信
组件通过 GFramework 事件通信，而非 Godot 信号：
- **发送：** `this.SendEvent(new SomeEvent { ... })`（触发所有已注册的处理程序）
- **订阅：** 在 `RegisterEvent()` 内使用 `this.RegisterEvent<SomeEvent>(e => { ... })`，并以 `.UnRegisterWhenNodeExitTree(this)` 链式调用确保节点退出时自动注销
- 事件位于 `scripts/cqrs/<domain>/event/`，命名空间 `...cqrs.<domain>.@event`
- 命令位于 `scripts/cqrs/<domain>/command/`，命令输入位于 `scripts/cqrs/<domain>/command/input/`
- 命令发送：`this.SendCommand(new SomeCommand(input))`

### 组件分类

| 目录 | 职责 | 有无 I* 接口 |
|---|---|---|
| `scripts/component/` | 可复用游戏组件（Calculator、Deck、Selector、TimeBar 等） | 有 |
| `scripts/entities/` | 领域实体（Poker 及其状态、状态机） | 有 |
| `scripts/menu/` | UI 页面（MainMenu、CalculateMenu、OptionsMenu、Credits） | 无（由 UiRouter 管理） |

### 扑克状态机
每张牌运行 4 状态 FSM：`Idle` ↔ `UnSelect` ↔ `OnSelect`，外加 `Drag`（鼠标按下时从 Idle 进入，鼠标释放时退出到 Idle）。状态变更通过 `PokerStateChangedEvent` 分发。

### 选择器
FIFO 队列，有容量限制。队列满时淘汰最早选中的牌。`Pop()` 为 LIFO（撤销最近一次选择）。通过 `SelectorEnableChangedEvent` 控制启用/禁用。响应 `SelectorSelectChangedEvent`。

### 日志与上下文
- `[Log]` 特性通过 GFramework 源代码生成器自动生成静态 `Log` 属性
- `[ContextAware]` 特性自动注入 GFramework 架构上下文
- 两者**成对使用**，`[Log]` 在前

## 核心游戏逻辑

**计算器组件** (`scripts/component/calculator/`) — `Calculator` 管理 12 种运算模式，每种模式实现 `IMode` 接口。

- **Fraction** (`scripts/model/calculator/Fraction.cs`) — 精确有理数结构体（GCD 约分、连分数转换 double）
- **Mode 抽象基类** (`scripts/component/calculator/mode/Mode.cs`) — 共享 Fraction 解析与数值格式化工具
- **二元 Mode（7 种）：** Add、Subtract、Multiply、Divide、Modulo、Power、NthRoot
- **一元 Mode（5 种）：** AbsoluteValue、Factorial、SquareRoot、Ceil、Floor
- **输入：** `IPoker` 对象（读取 `NumValue` 字符串 + `NumType` 枚举）
- **输出：** 字符串 — 数字或 `"ERROR:DivByZero"` / `"ERROR:ZeroRootIndex"` / `"ERROR:InvalidSqrt"` / `"ERROR:NoModeSelected"` / `"ERROR:InsufficientHands"`

## 当前开发状态

项目遵循 Plan.md 的 5 阶段路线图。**阶段三（核心游戏循环）** 是当前活跃阶段：

### 已完成
- `CalculateMenu` 12 个运算符按钮全部接入 `Calculator.ChangeTo(modeType)`，通过数据驱动循环绑定（`foreach Enum.GetValues<ModeType>()`）
- `Calculator` 已监听 `DeckHandCheckedEvent`，根据当前 Mode 的类型（二元/一元）执行计算并发送 `CalculatorResultEvent`（含错误状态码）
- `Selector` 具备启用/禁用功能，通过 `SelectorEnableChangedEvent` 控制
- `Deck` 支持按花色/点数排序和拖拽插入卡槽
- CalculateMenu 含临时测试脚手架：`CreateTest()` 创建 4 张测试牌（20、4、6、8）+ 120 秒计时器（标注 TODO）
- CQRS 层已统一编码规范：全部事件/命令 `sealed`，属性 `init` + `required`，命名空间一致

### 待完成
- 核心循环"选牌 → 选择运算 → 计算 → **验证结果(=24?) → 结算**"中，验证与结算环节**尚未接通**
- `DeckDiscardCheckedEvent` 暂无订阅者
- 测试脚手架 `CreateTest()` 需替换为正式的 `IRunManager` 关卡管理器
- 阶段一（牌组预设系统）尚未启动，`SuitType` 仅有 4 种基础花色
- 阶段二、四、五尚未开始
