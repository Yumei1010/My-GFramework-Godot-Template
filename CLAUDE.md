# CLAUDE.md

本文档为 Claude Code (claude.ai/code) 在本仓库中工作时提供指导。

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
| `*.cs` | 核心：`_Ready()` 调用 `ReadyAsync()`、`ConnectSignal()`、`RegisterEvent()` |
| `*.Dependencies.cs` | Godot 节点引用（`%NodeName`）、异步初始化逻辑 |
| `*.Properties.cs` | 字段、`UiKeyStr` |
| `*.Events.cs` | 通过 `RegisterEvent()` 订阅 CQRS 事件 |
| `*.Signals.cs` | Godot 信号 → CQRS 事件转换（`ConnectSignal()`） |

**Entity partial 类** 遵循相同模式：`Entity.cs`、`Entity.Dependencies.cs`、`Entity.Properties.cs`、`Entity.Events.cs`、`Entity.Signals.cs`。

## 核心模式

### CQRS 通信
组件通过 GFramework 事件通信，而非 Godot 信号：
- **发送：** `this.SendEvent(new SomeEvent { ... })`（触发所有已注册的处理程序）
- **订阅：** 在 `RegisterEvent()` 内使用 `this.RegisterEvent<SomeEvent>(e => { ... })`
- Entity 事件位于 `scripts/cqrs/*/event/`；命令位于 `scripts/cqrs/*/command/`

### 扑克状态机
每张牌运行 4 状态 FSM：`Idle` ↔ `UnSelect` ↔ `OnSelect`，外加 `Drag`（鼠标按下时从 Idle 进入，鼠标释放时退出到 Idle）。状态变更通过 `PokerStateChangedEvent` 分发。

### 选择器
FIFO 队列，有容量限制。队列满时淘汰最早选中的牌。`Pop()` 为 LIFO（撤销最近一次选择）。响应 `SelectorSelectChangedEvent`。

### 日志与上下文
- `[Log]` 特性通过 GFramework 源代码生成器自动生成静态 `Log` 属性
- `[ContextAware]` 特性自动注入 GFramework 架构上下文

## 核心游戏逻辑

**计算器组件** (`scripts/component/calculator/`) — `Calculator` 管理 12 种运算模式，每种模式实现 `IMode` 接口。

- **Fraction** (`scripts/model/calculator/Fraction.cs`) — 精确有理数结构体（GCD 约分、连分数转换 double）
- **Mode 抽象基类** (`scripts/component/calculator/mode/Mode.cs`) — 共享 Fraction 解析与数值格式化工具
- **二元 Mode（7 种）：** Add、Subtract、Multiply、Divide、Modulo、Power、NthRoot
- **一元 Mode（5 种）：** AbsoluteValue、Factorial、SquareRoot、Ceil、Floor
- **输入：** `IPoker` 对象（读取 `NumValue` 字符串 + `NumType` 枚举）
- **输出：** 字符串 — 数字或 `"ERROR:DivByZero"` / `"ERROR:ZeroRootIndex"` / `"ERROR:InvalidSqrt"`

## 当前开发状态

项目遵循 Plan.md 的 5 阶段路线图。**阶段三（核心游戏循环）** 是当前活跃阶段：

- `CalculateMenu` 创建 4 张测试牌（20、4、6、8）并启动 120 秒计时器——此为临时脚手架
- `DeckHandCheckedEvent` 和 `DeckDiscardCheckedEvent` 已发送但**没有订阅者**
- `CalculateBar` 有 12 个操作按钮（全部 `ModeType` 运算）但**点击处理函数为空**
- 核心循环"选牌 → 选择运算 → 计算 → 验证结果"**尚未接通**
- 阶段一、二、四、五尚未开始
