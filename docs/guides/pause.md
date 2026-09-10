# 暂停栈系统教程（Pause Stack）

> 本文纯教学：讲清"为什么需要分组暂停栈 + 怎么用 GFramework 暂停系统"。
> 框架已带完整实现（`PauseStackManager` + `PauseGroup` + `PauseToken` + `GodotPauseHandler`），无需自造轮子。
> 模板尚未接入（无 `PauseStackManager` 注册、无 `GodotPauseHandler` 注册）——本文含完整接入指南。

---

## 0. 一句话先说明白

**暂停栈 = 分组引用计数的暂停系统，比 Godot 原生单 `SceneTree.Paused` 强得多。**

```
玩家打开设置菜单 → Push(Gameplay 组, reason: "UI:Options")
   → Gameplay 组暂停（游戏逻辑冻结，UI/动画仍可动）
关闭菜单 → Pop(token)
   → Gameplay 组恢复
```

- **多组**：Global / Gameplay / Animation / Audio / Custom1-3 互不干扰
- **嵌套**：同组可多层 Push（引用计数），最后一层 Pop 才真正恢复
- **令牌制**：Push 返回 token，Pop 凭 token——**任意顺序释放**
- **可诊断**：`GetPauseReasons()` 能查"谁在暂停"

---

## 1. 为什么需要（对比 Godot 原生）

| 场景 | Godot `Tree.Paused` | 暂停栈 |
|---|---|---|
| 游戏内菜单暂停 | ✅ 全局暂停 | ✅ 且可分"只停玩法、UI 还动" |
| 对话时暂停战斗、但动画继续 | ❌ 做不到（全局一刀切） | ✅ Gameplay 组暂停 |
| 暂停下还想播音频 | ❌ 全局静音 | ✅ Audio 组独立 |
| 记不清谁暂停的 | ❌ 无从查 | ✅ `GetPauseReasons()` |
| 多层弹窗/嵌套暂停 | 手动计数，易错 | ✅ 栈深 `GetPauseDepth()` |

---

## 2. 核心概念

```
IPauseStackManager
 ├─ 每个 PauseGroup 一个 Stack<PauseEntry>
 ├─ Push(reason, group) → PauseToken   （入栈，记录原因）
 ├─ Pop(token) → bool                  （凭令牌弹出）
 ├─ PauseScope(reason, group) → IDisposable  （using 语法自动 Push/Pop）
 ├─ IsPaused(group) / GetPauseDepth(group) / GetPauseReasons(group)
 ├─ RegisterHandler(IPauseHandler)     （组状态变化通知）
 └─ OnPauseStateChanged 事件
```

### PauseGroup（分组）

```csharp
public enum PauseGroup
{
    Global, Gameplay, Animation, Audio,     // 内置
    Custom1, Custom2, Custom3               // 自定义扩展
}
```

### PauseToken（令牌）

```csharp
readonly struct PauseToken { Guid Id; bool IsValid; }   // Push 返回；Pop 凭它
```

### 通知机制

组从"未暂停→暂停"或"暂停→未暂停"**边界**时，通知所有注册的 `IPauseHandler`（按 `Priority` 排序）+ 触发 `OnPauseStateChanged`。

---

## 3. 基本用法

```csharp
using GFramework.Core.Abstractions.Pause;

var manager = new PauseStackManager();

// 暂停（返回令牌，必须保存）
var token = manager.Push("打开设置", PauseGroup.Gameplay);
manager.IsPaused(PauseGroup.Gameplay);   // true
manager.GetPauseDepth(PauseGroup.Gameplay);  // 1

// 恢复
manager.Pop(token);
manager.IsPaused(PauseGroup.Gameplay);   // false
```

### PauseScope（推荐：using 自动管理）

```csharp
using (manager.PauseScope("结算中", PauseGroup.Gameplay))
{
    // 这里 Gameplay 组暂停
    await DoSettlementAsync();
}   // 离开作用域自动 Pop —— 防漏
```

### 嵌套暂停（引用计数）

```csharp
var a = manager.Push("弹窗A");
var b = manager.Push("弹窗B");
manager.GetPauseDepth(PauseGroup.Global);   // 2
manager.Pop(a);                              // 弹栈 B 的? 见"任意顺序"
manager.IsPaused(PauseGroup.Global);         // 仍 true（b 还在）
manager.Pop(b);
manager.IsPaused(PauseGroup.Global);         // false（最后一个 Pop 才恢复）
```

> **同组内"任意顺序释放"**：`Pop(token)` 支持移除非栈顶令牌（内部临时转移栈恢复顺序）——生命周期乱序也不会错。

### 诊断

```csharp
manager.GetPauseReasons(PauseGroup.Global);   // ["弹窗B", "弹窗A"] 之类 —— 明明白白谁暂停的
manager.IsPaused();                           // Global 默认
```

---

## 4. Godot 桥接（关键认知）

### GodotPauseHandler 只桥接 Global 组

```csharp
using GFramework.Godot.Pause;

var handler = new GodotPauseHandler(GetTree());
manager.RegisterHandler(handler);
// → Global 组暂停时 SceneTree.Paused = true；恢复时 false
```

**⚠️ 框架刻意边界：`GodotPauseHandler` 只处理 `PauseGroup.Global`！** Gameplay/Animation/Audio/Custom 组**不会**自动影响 Godot。

需要"暂停时冻结该类对象"的项目，自行注册 handler：

```csharp
public sealed class GameplayPauseHandler : IPauseHandler
{
    public int Priority => 0;
    public void OnPauseStateChanged(PauseGroup group, bool isPaused)
    {
        if (group == PauseGroup.Gameplay)
            ApplyToNodes(isPaused ? freezeAllFight : resumeAllFight);
    }
}
manager.RegisterHandler(new GameplayPauseHandler());
```

**Godot 节点侧约定**：
- 想"暂停时仍动"的节点 → `ProcessMode.WhenPaused`
- 想"暂停时冻结"的保持默认（`ProcessMode.Pausable`）
- 想自定义组控制的计时器/音频 → 挂接对应组的 handler

---

## 5. UI 联动（最实用）

页面 `InteractionProfile` 声明 `PauseMode = WhileVisible` → **页面可见期间自动 Push 暂停，隐藏自动 Pop**（`UiRouterBase` 已内置）：

```csharp
public UiInteractionProfile InteractionProfile => new()
{
    PauseMode = UiPauseMode.WhileVisible,     // 可见期间持暂停
    PauseGroup = PauseGroup.Global,           // 或 Gameplay（只停玩法）
    PauseReason = "UI:OptionsMenu"            // 诊断可读
};
```

前置条件：路由能在容器查到 `IPauseStackManager`（`TryBindPauseStackManager`），查不到则暂停联动静默禁用。

**这就是"游戏内菜单自动暂停游戏"的标准姿势**：设置/背包/结算弹窗打开 → 自动暂停，关闭 → 恢复，不用手写。

---

## 6. 模板接入指南（5 步）

| 步骤 | 内容 |
|---|---|
| 1 | 注册 `PauseStackManager`：`architecture.RegisterUtility(new PauseStackManager());` |
| 2 | 注册 `GodotPauseHandler`：`manager.RegisterHandler(new GodotPauseHandler(GetTree()));`（Global 组 ↔ SceneTree.Paused 桥） |
| 3 | 自定义组监听：需要时注册 Gameplay/Animation 组 handler（控制 ProcessMode/计时器/音频） |
| 4 | Modal 类 UI 页面 `InteractionProfile` 设 `WhileVisible` + 目标组 |
| 5 | （模板既有）`GameInputController` 的 Paused 阶段判定依赖 `Tree.Paused`——接线后由框架驱动 |

> 模板 `GlobalInputController` 现在读 Godot 原生 `Tree.Paused` 判断输入阶段——接入后 Global 组暂停会正确驱动它。

---

## 7. 常见坑

| 坑 | 说明 |
|---|---|
| **Pop 用错 token** | `Pop` 凭 token，不凭 reason；token 是 Guid，重复 Pop 返回 false（幂等安全） |
| **忘把 token 存下来** | Push 的返回值必须保存，否则无法 Pop（用 `PauseScope` 避免） |
| **只 Register Global handler** | 其它组不会自动让 Godot 冻结——要自注册 handler |
| **服务被销毁后用** | Destroy 后 Push 会抛 `ObjectDisposedException` |
| **handler 里做耗时操作** | 通知在锁外快照派发，但仍是调用线程；别在通知里阻塞 |

---

## 8. API 速查

| API | 说明 |
|---|---|
| `manager.Push(reason, group = Global)` → `PauseToken` | 暂停（返回令牌） |
| `manager.Pop(token)` → `bool` | 凭令牌恢复（任意顺序） |
| `manager.PauseScope(reason, group)` → `IDisposable` | using 自动 Push/Pop |
| `manager.IsPaused(group)` / `GetPauseDepth(group)` | 状态 / 嵌套层数 |
| `manager.GetPauseReasons(group)` | 暂停原因列表（诊断） |
| `manager.ClearGroup(group)` / `ClearAll()` | 强制清空（慎用） |
| `manager.RegisterHandler(IPauseHandler)` / `UnregisterHandler` | 组状态通知 |
| `manager.OnPauseStateChanged` | 事件（含 group + isPaused） |
| `new GodotPauseHandler(SceneTree)` | Global 组 → `Tree.Paused` 桥 |
| `UiInteractionProfile.PauseMode/PauseGroup/PauseReason` | UI 页面自动暂停 |

---

## 9. 源码阅读入口

| 路径 | 内容 |
|---|---|
| `GFramework.Core/Pause/PauseStackManager.cs` | 核心（分栈/令牌/任意顺序释放/通知/销毁补发） |
| `GFramework.Core/Pause/PauseScope.cs` | using 作用域 |
| `GFramework.Core.Abstractions/Pause/` | 接口层（IPauseHandler/IPauseStackManager/PauseGroup/PauseToken） |
| `GFramework.Godot/Pause/GodotPauseHandler.cs` | Godot 桥（仅 Global 组） |
| `research/gframework-extensions/pause-stack.md` | 深度研究（含 UI 联动细节） |