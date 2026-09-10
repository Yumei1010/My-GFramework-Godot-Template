# GFramework 输入域深度研究

> 日期：2026-09 | 对应需求场景：手柄/键盘/触摸三端热切换
> 源码：`GFramework/GFramework.Game(.Abstractions)/Input/` + `GFramework.Godot/Input/` + `GFramework.Game(.Abstractions)/UI/` 输入相关

## 结论（TL;DR）

输入域是 **三层协议**，全部有完整实现，模板尚未接线：

1. **绑定层**（`IInputBindingStore`）：Godot InputMap ↔ 框架绑定描述双向适配。重绑定/快照导入导出/恢复默认/冲突自动交换
2. **设备层**（`IInputDeviceTracker`）：从原生输入事件推断当前设备（KeyboardMouse/Gamepad/Touch）
3. **UI 语义层**（`IUiInputActionMap` + `IUiInputDispatcher` + `InteractionProfile`）：物理键 → 逻辑动作 → **UI 语义动作（Cancel/Confirm）** → 路由仲裁 → 页面处理

**架构主张**：业务代码不接触原生输入事件，只依赖动作名与 `UiInputAction` 语义。

## 一、绑定与设备层

### 值类型（Abstractions/Input）

| 类型 | 字段 | 说明 |
|---|---|---|
| `InputBindingDescriptor` | `DeviceKind` / `BindingKind` / `Code` / `DisplayName` / `AxisDirection?` | 一条绑定。Code 格式：`key:32` / `mouse:1` / `joy-button:0` / `joy-axis:0:1`（数字为 Godot 枚举值） |
| `InputBindingKind` | Key / MouseButton / GamepadButton / GamepadAxis | 绑定种类 |
| `InputDeviceKind` | KeyboardMouse / Gamepad / Touch | 设备类别 |
| `InputDeviceContext` | `DeviceKind` + `DeviceIndex?` + `DeviceName` | 设备上下文（手柄区分索引） |
| `InputActionBinding` | `ActionName` + `Bindings[]` | 一个动作的绑定集合 |
| `InputBindingSnapshot` | `Actions[]` | 全量绑定快照（可序列化存档） |

### 接口

```csharp
public interface IInputBindingStore
{
    InputActionBinding GetBindings(string actionName);
    InputBindingSnapshot ExportSnapshot();                    // 全量导出（存档/云同步）
    void ImportSnapshot(InputBindingSnapshot snapshot);        // 全量导入
    void SetPrimaryBinding(string actionName, InputBindingDescriptor binding, bool swapIfTaken = true); // 设主绑定，冲突可交换
    void ResetAction(string actionName);                       // 恢复单动作默认
    void ResetAll();                                           // 全部恢复默认
}

public interface IInputDeviceTracker
{
    InputDeviceContext CurrentDevice { get; }                  // 当前活跃设备
}
```

### 实现层

| 类 | 位置 | 职责 |
|---|---|---|
| `InputBindingStore` | Game/Input（纯逻辑 192 行） | 内存态绑定管理：默认快照记忆、冲突交换算法（把被占动作的原绑定换给新动作）、快照导入导出。**不依赖宿主，可单测** |
| `InputDeviceTracker` | Game/Input | `Update(context)` 单线程更新当前设备 |
| `GodotInputBindingStore` | Godot/Input | **公开入口**：`IInputBindingStore + IInputDeviceTracker` 双实现。每次操作先 `ReloadFromBackend()` 与 Godot InputMap 对齐，写操作后回写 InputMap |
| `GodotInputMapBackend` | Godot/Input（internal） | InputMap 读写 + 捕获默认快照；测试注入点 |
| `GodotInputBindingCodec` | Godot/Input（internal static） | 原生 InputEvent ↔ `InputBindingDescriptor` 双向转换 + `GetDeviceContext(InputEvent)` 设备推断 |

**设备更新用法**（关键，需你自己接在输入入口）：

```csharp
// 在节点 _Input / _UnhandledInput 里，把每个事件喂给 store 即可维护"当前设备"
inputStore.UpdateDeviceFromInput(inputEvent);  // GodotInputBindingStore 自带
// 之后 UI 层可查 inputStore.CurrentDevice.DeviceKind == InputDeviceKind.Gamepad
```

## 二、UI 语义动作层

### 值类型（Abstractions/UI）

```csharp
public enum UiInputAction { None, Cancel, Confirm }        // 语义动作（仅两个，刻意收敛）
[Flags] public enum UiInputActionMask { None, Cancel = 1, Confirm = 2 }

public sealed class UiInteractionProfile
{
    public UiInputActionMask CapturedActions { get; init; }   // 页面声明捕获哪些语义动作
    public bool BlocksWorldPointerInput { get; init; }        // 阻断世界指针输入
    public bool BlocksWorldActionInput { get; init; }         // 阻断世界语义动作输入
    public UiPauseMode PauseMode { get; init; }               // 与暂停栈联动
    public PauseGroup PauseGroup { get; init; }               // 暂停分组
    public bool ContinueProcessingWhenPaused { get; init; }
    public string PauseReason { get; init; }
}
```

### 链路

```
物理输入 (InputEvent)
  → GodotInputBindingCodec.GetDeviceContext → InputDeviceTracker（设备感知）
  → 项目代码检测 Input.IsActionPressed("ui_cancel" / "confirm" / 自定义动作)
  → UiInputActionMap.TryMap("ui_cancel" → UiInputAction.Cancel)   // 默认映射已含 ui_cancel/ui_accept 别名
  → UiInputDispatcher.TryDispatch("ui_cancel")
       → UiRouterBase.TryDispatchUiAction(Cancel)
            → GetUiActionOwner(action)：按 Topmost→Modal→Overlay→Page→Toast 优先级
              找第一个 InteractionProfile 声明捕获该动作的可见页面
            → 命中页面若实现 IUiActionHandler.TryHandleUiAction(action) 则回调
```

**页面侧接线**（模板页面模式基础上）：

```csharp
// IUiPageBehavior.InteractionProfile 返回页面声明（例：模态弹窗）
public UiInteractionProfile InteractionProfile =>
    UiInteractionProfiles.BlockingCancel;   // 预制：捕获 Cancel + 阻断 World 双输入

// 页面视图实现 IUiActionHandler
public bool TryHandleUiAction(UiInputAction action) =>
    action == UiInputAction.Cancel ? HandleCancel() : false;
```

**预制档案**（`UiInteractionProfiles`）：
- `Default`：不捕获、不阻断
- `BlockingCancel`：捕获 Cancel + 阻断 World 指针/动作输入
- `CreateDefault(UiLayer)`：Modal/Topmost → BlockingCancel；其余 → Default

**路由辅助判定**（供游戏世界查询"当前 UI 是否挡住输入"）：
- `UiRouterBase.BlocksWorldPointerInput()` / `BlocksWorldActionInput()`

## 三、模板现状 gap

| 项 | 模板现状 | 需要补 |
|---|---|---|
| `GodotInputBindingStore` 注册 | 未注册 | 注册进 UtilityModule（`RegisterUtility`），或作为框架系统 |
| 设备更新喂入 | `GlobalInputController._Input` 只处理 ui_cancel | 每事件 `UpdateDeviceFromInput` |
| 语义动作映射 | 无 | 注册 `IUiInputActionMap`（默认实现够用） |
| 页面 InteractionProfile | 页面无 profile 概念 | 页面类加属性（Modal 类页面用 BlockingCancel） |
| 键盘焦点导航 | Godot 原生 focus 机制 | 表现层规范（focus 链 + 触摸末次输入隐藏焦点环） |

## 四、与三端输入热切换需求的关系

热切换玩法本质拆解为：
1. **设备感知**：`IInputDeviceTracker` → 触摸时隐藏焦点环/显示触控 UI、手柄时显示按键提示
2. **语义动作**：Selector 选牌/确认/取消 → `UiInputAction`（不再绑物理键）
3. **焦点驱动**：页面键盘/手柄导航走 Godot focus + `ui_*` 动作（已映射），鼠标/触摸走原生点击

**Selector 双路径**（点按 vs 焦点）可直接用这套：点按路径不变；焦点路径 = Godot focus 移动 + 页面实现 `IUiActionHandler` 接收 Confirm 触发选牌。

## 五、可测性

- `InputBindingStore` 纯逻辑可单测（冲突交换/快照/重置基线已有框架测试模式）
- `UiInputDispatcher`/`UiInputActionMap` 依赖 `IUiRouter`/map，可注入 fake 单测
- 模板沉淀组件时可参考 GFramework `GFramework.Game.Tests` 输入测试写法

## 相关文档

- 框架 `docs/zh-CN/game/`（Game 层）与 `docs/zh-CN/core/` 下的 input/ui 文档
- 扩展接口总览：`docs/research/gframework-extensions/README.md`
