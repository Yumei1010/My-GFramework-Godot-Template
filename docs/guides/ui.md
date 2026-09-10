# UI 路由与弹窗教程（UI Router）

> 本文纯教学：讲清"UI 页面/弹窗怎么管理 + 交互语义怎么配"。
> 框架提供完整 UI 路由（页面栈 + 分层管理 + 过渡管道 + 输入语义 + 守卫），模板已接入（`UiRouter`/`UiFactory`/`ISimpleUiPage`）。
> 配套阅读：[pause.md](pause.md)（弹窗自动暂停）、[input-domain 研究](../research/gframework-extensions/input-domain.md)（语义动作）。

---

## 0. 一句话先说明白

**UI 路由 = 页面栈（顺序导航）+ 分层管理（Overlay/Modal/Toast 浮层）+ 输入语义（Cancel/Confirm 仲裁）+ 过渡管道 + 路由守卫。**

弹窗不需要自己 `AddChild` —— 交给 `UiRouter` 管生命周期、层级、输入阻断与暂停联动。

---

## 1. 为什么需要（手写弹窗的痛）

| 问题 | 手写 | UI 路由 |
|---|---|---|
| 弹窗叠层顺序 | 靠 `z_index`/添加顺序 | `UiLayer` 五层语义 + 优先级仲裁 |
| 下层输入被穿透 | 手动 `set_process_input(false)` | `BlocksWorldPointerInput/ActionInput` 声明式阻断 |
| Esc 关闭哪个弹窗 | 手动判断"最上层" | `TryDispatchUiAction(Cancel)` 自动找捕获者 |
| 弹窗打开要暂停游戏 | 手写 Tree.Paused | `InteractionProfile.PauseMode` 自动暂停 |
| 转场动画 | 到处写 Tween | `IUiTransitionHandler` 管道统一 |

---

## 2. 核心概念

### UiLayer（五个层级）

```csharp
public enum UiLayer { Page, Overlay, Modal, Toast, Topmost }
```

| 层 | 用途 | 输入优先级 |
|---|---|---|
| `Page` | 主页面（页面栈，Push/Pop） | 最低（栈内） |
| `Overlay` | 覆盖层（HUD/浮动面板，不挡下层） | 中 |
| `Modal` | 模态弹窗（挡输入，通常暂停） | 较高 |
| `Toast` | 提示条（短暂、不挡输入） | 最低 |
| `Topmost` | 最高优先（紧急弹窗/调试） | 最高 |

**输入仲裁顺序**：`Topmost → Modal → Overlay → Page(栈顶→底) → Toast`

### 两种 UI 容器

| 容器 | API | 特点 |
|---|---|---|
| **页面栈** | `PushAsync` / `PopAsync` / `ReplaceAsync` / `ClearAsync` | 有先后顺序，同 key 不重复入栈 |
| **层管理** | `Show` / `Hide` / `Resume` / `ClearLayer` | 无顺序（层内并列），返回 `UiHandle` |

### 过渡策略与弹出策略

```csharp
public enum UiTransitionPolicy { Exclusive, Overlay }   // Exclusive: 下层 Pause+Suspend；Overlay: 仅 Pause
public enum UiPopPolicy { Destroy, Suspend }            // 弹出时销毁 或 挂起（保留实例）
```

### UiHandle（层 UI 句柄）

```csharp
readonly struct UiHandle { string InstanceId; string Key; UiLayer Layer; }
```

---

## 3. 基本用法

### 页面栈导航

```csharp
var uiRouter = this.GetSystem<IUiRouter>()!;

await uiRouter.PushAsync(nameof(UiKey.MainMenu));            // 压栈（独占：下层挂起）
await uiRouter.PushAsync(nameof(UiKey.Settings), policy: UiTransitionPolicy.Overlay);  // 覆盖（下层仅暂停）
await uiRouter.PopAsync();                                   // 弹栈（Destroy）
await uiRouter.PopAsync(UiPopPolicy.Suspend);                // 弹栈但保留实例
await uiRouter.ReplaceAsync(nameof(UiKey.Game));             // 清栈换页
await uiRouter.ClearAsync();                                 // 清空
```

### 分层 UI（弹窗/HUD/提示）

```csharp
// 显示模态弹窗（返回句柄）
UiHandle handle = uiRouter.Show(nameof(UiKey.ConfirmDialog), UiLayer.Modal);

// 显示提示条
var toast = uiRouter.Show(nameof(UiKey.Toast), UiLayer.Toast);

// 隐藏（destroy: 是否销毁）
uiRouter.Hide(handle, UiLayer.Modal, destroy: false);   // 挂起（保留）
uiRouter.Resume(handle, UiLayer.Modal);                  // 恢复显示
uiRouter.ClearLayer(UiLayer.Toast, destroy: true);       // 清空整层

// 查询
uiRouter.HasVisibleInLayer(handle, UiLayer.Modal);
uiRouter.GetAllFromLayer(nameof(UiKey.Toast), UiLayer.Toast);
```

> ⚠️ `Show` 不能用于 `UiLayer.Page`（要抛异常）——页面栈用 `PushAsync`。

---

## 4. 弹窗交互语义（InteractionProfile，重点）

页面通过 `InteractionProfile` **声明式**告诉路由：我捕获哪些语义动作、是否阻断世界输入、是否暂停游戏。

```csharp
public sealed class UiInteractionProfile
{
    public UiInputActionMask CapturedActions { get; init; }        // 捕获 Cancel/Confirm
    public bool BlocksWorldPointerInput { get; init; }             // 阻断世界指针输入
    public bool BlocksWorldActionInput { get; init; }              // 阻断世界语义动作输入
    public UiPauseMode PauseMode { get; init; }                    // None / WhileVisible
    public PauseGroup PauseGroup { get; init; }
    public string PauseReason { get; init; }
}
```

### 预置档案（`UiInteractionProfiles`）

| 档案 | 语义 |
|---|---|
| `Default` | 不捕获、不阻断 |
| `BlockingCancel` | 捕获 Cancel + 阻断世界双输入（模态弹窗标配） |
| `CreateDefault(layer)` | `Modal`/`Topmost` → BlockingCancel；其余 → Default |

### 弹窗里响应 Esc / 确认键

```csharp
// 1. 声明档案（模态弹窗）
public UiInteractionProfile InteractionProfile => UiInteractionProfiles.BlockingCancel;

// 2. 页面视图实现 IUiActionHandler 接收语义动作
public bool TryHandleUiAction(UiInputAction action)
{
    if (action != UiInputAction.Cancel) return false;
    this.SendCommand(new CloseConfirmDialogCommand());   // Esc → 关弹窗
    return true;
}
```

### 游戏世界查询"UI 是否挡住输入"

```csharp
if (uiRouter.BlocksWorldPointerInput()) return;   // 弹窗开着就别响应场景点击
if (uiRouter.BlocksWorldActionInput()) return;
```

### 与暂停联动（打开弹窗自动暂停游戏）

```csharp
public UiInteractionProfile InteractionProfile => new()
{
    CapturedActions = UiInputActionMask.Cancel,
    BlocksWorldPointerInput = true,
    BlocksWorldActionInput = true,
    PauseMode = UiPauseMode.WhileVisible,     // 可见期间自动 Push 暂停
    PauseGroup = PauseGroup.Global,           // 或 Gameplay（只停玩法）
    PauseReason = "UI:ConfirmDialog"
};
// → 弹窗显示=暂停，关闭=恢复（详见 pause.md）
```

---

## 5. UI 页面骨架（模板模式）

模板用 partial class + 语法糖，弹窗页面同样写法：

```csharp
[Log]
[ContextAware]
[AutoUiPage(nameof(UiKey.ConfirmDialog), nameof(UiLayer.Modal))]   // ← 层决定它是弹窗
public partial class ConfirmDialog : Control, IController, IUiPageBehaviorProvider, ISimpleUiPage
{
    [GetNode] private Button _okButton = null!;
    [GetNode] private Button _cancelButton = null!;

    public override void _Ready()
    {
        __InjectGetNodes_Generated();
        __BindNodeSignals_Generated();
        _ = ReadyAsync();
        RegisterEvents();
    }

    // 模态弹窗交互档案
    public UiInteractionProfile InteractionProfile => UiInteractionProfiles.BlockingCancel;

    public bool TryHandleUiAction(UiInputAction action)
    {
        if (action == UiInputAction.Cancel) { _cancelButton.EmitSignal(Button.SignalName.Pressed); return true; }
        return false;
    }
}
```

> 页面不需要自己 `AddChild`/`QueueFree`——路由通过 `IUiRoot` 管理挂载与销毁。

---

## 6. 路由守卫（IUiRouteGuard）

进入/离开页面前做业务检查：

```csharp
public sealed class UnsavedSettingsGuard : IUiRouteGuard
{
    public ValueTask<bool> CanEnterAsync(string uiKey, IUiPageEnterParam? param)
        => ValueTask.FromResult(true);

    public ValueTask<bool> CanLeaveAsync(string uiKey)
    {
        // 返回 false → 阻止离开（例如"未保存设置"提示）
        return ValueTask.FromResult(!HasUnsavedChanges());
    }
}
```

适合：未保存拦截、新手引导期间禁用跳转、多层弹窗切换确认。

---

## 7. 过渡处理器（IUiTransitionHandler）

转场动画/日志/埋点统一挂管道：

```csharp
uiRouter.RegisterHandler(new LoggingTransitionHandler());               // 日志（模板已用）
uiRouter.RegisterHandler(new FadeTransitionHandler(),                    // 淡入淡出
    new UiTransitionHandlerOptions(TimeoutMs: 2000, ContinueOnError: true));
uiRouter.UnregisterHandler(handler);

// Around 处理器（包裹整个转场过程）
```

> 文档建议：转场逻辑放 Handler，**别把 UiRouter 派生类做成巨型协调器**。

---

## 8. Godot 层行为基类（可选）

每层有现成 `UiPageBehaviorBase`：`PageLayerUiPageBehavior<T>` / `OverlayLayerUiPageBehavior<T>` / `ModalLayerUiPageBehavior<T>`（`IsReentrant = true`）/ Toast / Topmost。

一般用语法糖 `[AutoUiPage(key, layer)]` 自动配好；要自定义行为时用 `UiPageBehaviorFactory.Create<T>(owner, key, layer)`。

---

## 9. 常见坑

| 坑 | 说明 |
|---|---|
| **`Show` 传 Page 层** | 抛异常——页面栈用 `PushAsync` |
| **非重入页面重复 Show** | 同 key 非 `IsReentrant` 会在同层重复时抛 `InvalidOperationException` |
| **忘了配 InteractionProfile** | 弹窗不阻断输入、Esc 不生效（默认档案什么都不捕获） |
| **阻断世界输入但没查** | 路由只提供 `BlocksWorld*` 查询，**游戏侧要自己查**才真正阻断 |
| **暂停没生效** | 需先注册 `IPauseStackManager`（见 pause.md），否则暂停联动静默禁用 |
| **同 key 重复 Push** | `IsTop` 时会忽略（记 Warn），不是错误 |

---

## 10. API 速查

| API | 说明 |
|---|---|
| `PushAsync(uiKey, param?, policy)` / `PopAsync(policy)` / `ReplaceAsync` / `ClearAsync` | 页面栈导航 |
| `Show(uiKey/page, layer, param?)` → `UiHandle` | 层 UI 显示（非 Page 层） |
| `Hide(handle, layer, destroy?)` / `Resume(handle, layer)` / `ClearLayer(layer, destroy?)` | 层 UI 控制 |
| `HideByKey` / `GetFromLayer` / `GetAllFromLayer` / `HasVisibleInLayer` | 层查询 |
| `GetUiActionOwner(action)` / `TryDispatchUiAction(action)` | 语义动作仲裁分发 |
| `BlocksWorldPointerInput()` / `BlocksWorldActionInput()` | 世界输入阻断查询 |
| `RegisterHandler` / `UnregisterHandler` | 过渡处理器 |
| `PeekKey()` / `Peek()` / `IsTop(key)` / `Contains(key)` / `Count` | 栈状态查询 |
| `IUiRouteGuard.CanEnterAsync/CanLeaveAsync` | 路由守卫 |

---

## 11. 源码阅读入口

| 路径 | 内容 |
|---|---|
| `GFramework.Game/UI/UiRouterBase.cs` | 路由核心（栈/层/仲裁/暂停联动/过渡管道） |
| `GFramework.Game/UI/UiInteractionProfiles.cs` | 交互档案预置与判定 |
| `GFramework.Game.Abstractions/UI/` | 接口（IUiRouter/IUiPageBehavior/IUiRouteGuard/…） |
| `GFramework.Godot/UI/` | Godot 层行为基类 + Factory |
| `scripts/core/ui/` | 模板 UiRouter / ISimpleUiPage / UiFactory |
| `docs/research/gframework-extensions/input-domain.md` | 语义动作深度研究 |
| `docs/guides/pause.md` | 弹窗自动暂停 |

---

## 附：语义验证测试

`tests/.../UiInteractionTests.cs` 覆盖交互档案语义（纯逻辑可测）：

| 用例 | 验证内容 |
|---|---|
| 预置档案_Default 不捕获不阻断 | `UiInteractionProfiles.Default` |
| 预置档案_BlockingCancel 捕获并阻断 | 捕获 Cancel + 双阻断 |
| CreateDefault_Modal与Topmost用阻塞档案 | 层级默认语义 |
| CreateDefault_其余层用默认档案 | Page/Overlay/Toast |
| Captures_按动作判定 | `Captures(profile, action)` |