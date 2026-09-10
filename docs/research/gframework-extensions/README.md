# GFramework 0.7.1 预留扩展接口研究

> 日期：2026-09 | 来源：GFramework 源码全量盘查（`E:\project\GitHub\GFramework`）
> 用途：日后开发速查——哪些扩展点开箱即用、哪些留空待实现、哪些是刻意边界

## 结论（TL;DR）

框架架构 = **抽象接口集中在 Abstractions 层（Core/Game/Godot 三层实现）**。扩展点分三类：

1. **✅ 完整实现，只差接线**——直接注册/组合即可用（日志 appender 就是此类）
2. **⚠️ 接口留空，契约完整**——框架提供抽象与装配位，实现由业务填
3. **🏗 半成品/刻意边界**——归档话题揭示作者的设计取舍，不要逆着走

---

## 一、与你当前路线直接相关

### 1. 输入域（完整实现）🔥

**位置**：`GFramework.Game.Abstractions/Input/` + `GFramework.Game/Input/` + `GFramework.Godot/Input/`

| 接口/类 | 状态 | 能力 |
|---|---|---|
| `IInputBindingStore` / `GodotInputBindingStore` | ✅ | Godot InputMap 适配：动作重绑定、快照导出/导入、恢复默认 |
| `IInputDeviceTracker` / `InputDeviceTracker` / `GodotInputBindingStore`(兼实现) | ✅ | 当前活跃设备识别（Keyboard/Mouse/Gamepad/Touch） |
| `IUiInputActionMap` / `UiInputActionMap` | ✅ | 逻辑动作名 → UI 路由动作映射 |
| `IUiInputDispatcher` / `UiInputDispatcher` | ✅ | 动作 → `IUiRouter.TryDispatchUiAction` 桥接 |
| `InputBindingDescriptor/Kind/Snapshot`、`InputDeviceContext/Kind` | ✅ | 绑定与设备的值类型 |

**注意**：`IGodotInputMapBackend` 是 internal（测试注入用），公开入口是 `GodotInputBindingStore()`。

→ **对应玩法需求**：手柄/键盘/触摸热切换（Twenty-four 玩法方向③）——框架已备好地基。

### 2. 事件过滤（接口留空）

**位置**：`GFramework.Core.Abstractions/Events/IEventFilter.cs`

```csharp
public interface IEventFilter<in T>
{
    bool ShouldFilter(T eventData); // true = 阻止传递给监听器
}
```

事件总线触发前的**条件拦截钩子**。用途：按玩家/关卡/特性开关过滤事件是否派发。频段总线（模板已接）之上可再叠一层。

### 3. 存档/设置迁移（接口留空）🔥

**位置**：`GFramework.Game.Abstractions/Data/ISaveMigration.cs`、`Setting/ISettingsMigration.cs`

```csharp
public interface ISaveMigration<TSaveData> where TSaveData : class, IData
{
    int FromVersion { get; }
    int ToVersion { get; }
    TSaveData Migrate(TSaveData oldData);
}
```

契约完整：`FromVersion → ToVersion` 迁移链，配 `IVersionedData` 判断当前版本。用途：Twenty-four run 存档、模板设置结构升级。

### 4. Store / Redux 风格状态管理（完整实现）🔥

**位置**：`GFramework.Core.Abstractions/StateManagement/` + `GFramework.Core/StateManagement/`

| 能力 | 说明 |
|---|---|
| `IStore<TState>` / `Store<TState>` | 单向流状态树 |
| `IReducer<TState, TAction>` | 强类型 reducer |
| `IStoreMiddleware<TState>` | 中间件（日志/调试/遥测钩子） |
| `IStateSelector<TState,TView>` | 聚合状态投影 |
| `WithHistoryCapacity(n)` | **撤销/重做/时间旅行调试** |
| `WithActionMatching(...)` | action 匹配策略（精确/多态） |

构建：`StoreBuilder<TState>`（`Store<TState>.CreateBuilder()`）。文档：`docs/zh-CN/core/state-management.md`。适合战斗结算/复杂状态机，事件模型之外的另一选择。

---

## 二、通用扩展点（模板沉淀候选）

| 扩展点 | 位置 | 状态 | 说明 |
|---|---|---|---|
| `IArchitectureModule` | Core.Abstractions/Architectures | ✅ 模板在用 | 4 个 module |
| `IServiceModule` | 同上 | ✅ | 带 `ModuleName/Priority/IsEnabled` 的高阶模块变体 |
| `IGodotModule` / `AbstractGodotModule` | Godot/Architectures | ✅ | Godot 宿主模块基类 |
| `IArchitectureLifecycleHook` | Core.Abstractions/Architectures | ⚠️ 空接口 | `OnPhase(phase, arch)` 架构阶段钩子 |
| `IArchitecturePhaseListener` | 同上 | ⚠️ | phase 监听 |
| `ILogAppender` / `ILogFilter` / `ILogFormatter` | Core.Abstractions/Logging | ✅ 已在用 | 日志三件套 |
| `IPauseHandler` + `PauseStackManager` | Core.Abstractions/Pause + Core/Pause | ✅ | **分组暂停栈**（`PauseGroup` 分栈），比 Godot 单 bool 强 |
| `GodotPauseHandler` | Godot/Pause | ✅ | Godot 暂停桥接 |
| `ITimeProvider` | Core.Abstractions/Time | ✅ | `UtcNow` 抽象——可测试计时 |
| `ITimeSource` / `IYieldInstruction` / `ICoroutineStatistics` | Core.Abstractions/Coroutine | ✅ | 协程时间源（`GodotTimeSource` 在 Godot 层） |
| `IResourceLoader` / `IResourceReleaseStrategy` / `IResourceManager` | Core.Abstractions/Resource | ⚠️ 部分 | 释放策略留空，可做 LRU/引用计数 |
| `IRuntimeTypeSerializer` / `ISerializer` | Core.Abstractions/Serializer | ✅ Game 有实现 | 类型感知序列化 |
| `IBindableProperty` / `IReadonlyBindableProperty` | Core.Abstractions/Property | ✅ | 可绑定属性（MVVM 风） |
| `IObjectPoolSystem` / `IPoolableObject` / `IPoolableNode` / `AbstractNodePoolSystem` | Core.Abstractions/Pool + Godot | ✅ | 对象池（教学文档已写） |
| `ILocalizationManager` / `ILocalizationTable` / `ILocalizationFormatter` / `ILocalizationString` | Core.Abstractions/Localization | ✅ | 本地化（含 Plural/Conditional formatter、`LocalizationMap`） |
| `INumericDisplayFormatter` / `INumericFormatRule` | Core.Abstractions/Utility/Numeric | ⚠️ | 数字显示格式化（CompactNumber 已实现） |
| `IRegistry` / `IKeyValue` / `IHasKey` | Core.Abstractions | ✅ | 注册表系（模板已在用 registry） |
| `IConfigurationManager` | Core.Abstractions/Configuration | ✅ | 配置管理 |
| `ICqrsRuntime` / `IController` / `ICommandExecutor` / `IQueryExecutor` | Core.Abstractions | ✅ | CQRS 底座 |
| `IIocContainer` | Core.Abstractions/Ioc | ✅ | DI 容器（`Configurator` 覆盖点） |

---

## 三、半成品 / 刻意边界（归档话题）

来源：`GFramework/ai-plan/public/archive/`（框架作者的设计记录）

| 话题 | 结论 |
|---|---|
| `godot-logging-core-sink` | Godot 日志已进 Core appender 管线（模板会话日志组件正是接这条线） |
| `single-context-priority` | **单上下文优先**：多架构上下文并行不支持，是刻意边界，不要逆着用 |
| `microsoft-di-container-disposal` | 容器销毁语义收尾 |
| `runtime-generator-boundary` | 运行时/生成器边界界定 |

**逆着设计走 = 打框架**。识别到"刻意边界"时应换方案而不是硬扩。

---

## 四、文档索引（均有 zh-CN 教程）

`docs/zh-CN/`：`core/`（cqrs/model/command/context/ioc/state-management/functional/extensions）、`game/`、`godot/`、`ecs/`、`abstractions/`（core-abstractions / ecs-arch-abstractions）、`best-practices/`（architecture-patterns / error-handling / multiplayer / mobile-optimization / performance）、`tutorials/`、`source-generators/`

---

## 五、子文档索引与建议使用顺序（沉淀模板节奏）

| 子文档 | 内容 |
|---|---|
| `input-domain.md` | 输入三层协议（绑定/设备/UI 语义动作），三端热切换地基 |
| `pause-stack.md` | 分组引用计数暂停栈，UI 自动暂停联动 |
| `store.md` | Redux 风格状态管理（另有教学文档 `docs/guides/store.md`） |
| `game-services.md` | Config(YAML 配置)/Data(存档)/Storage/Resource/Coroutine/RichText/UI·Scene 扩展点 |

建议接入顺序：

1. **Config 配置系统**（数据驱动地基）→ 详见 `game-services.md`
2. **输入域**（三端热切换玩法需求）→ 详见 `input-domain.md`
3. **ISaveMigration + SaveRepository**（Twenty-four run 存档）→ 详见 `game-services.md`
4. **IPauseHandler 分组暂停栈 + ITimeProvider**（暂停组件地基）→ 详见 `pause-stack.md`
5. **Store**（Run/结算等聚合状态）→ 详见 `store.md` + `docs/guides/store.md`
