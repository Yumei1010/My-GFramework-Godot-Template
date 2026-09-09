# GFramework Pause 栈深度研究

> 日期：2026-09 | 源码：`GFramework.Core.Abstractions/Pause/` + `GFramework.Core/Pause/` + `GFramework.Godot/Pause/`
> 关联：countdown_timer / 暂停组件的地基；UI InteractionProfile 与暂停联动

## 结论（TL;DR）

Pause 栈是 **分组引用计数暂停**，比 Godot 原生单 `SceneTree.Paused` 强：

- **多组**：`PauseGroup`（Global/Gameplay/Animation/Audio/Custom1-3），互不干扰
- **嵌套**：同组可多层 Push（引用计数式），最后一层 Pop 才真正恢复
- **令牌制**：Push 返回 `PauseToken`（Guid），Pop 必须凭令牌——支持**任意顺序释放**（非栈顶也能移除）
- **原因可查**：每层带 reason，可诊断"谁在暂停"
- **通知机制**：组暂停/恢复时通知注册的 `IPauseHandler` + 触发事件
- **UI 联动**：页面 `InteractionProfile.PauseMode = WhileVisible` → 页面可见期间自动持有一个暂停请求

**模板现状**：完全未接线（无 `PauseStackManager` 注册、无 `GodotPauseHandler` 注册）。

## 一、类型与接口

### 值类型（Abstractions/Pause）

```csharp
public enum PauseGroup { Global, Gameplay, Animation, Audio, Custom1, Custom2, Custom3 }

public readonly struct PauseToken    // Guid 包装；Invalid = Guid.Empty；支持 == 比较
{
    public Guid Id { get; }
    public bool IsValid { get; }
}

public sealed class PauseStateChangedEventArgs  // OnPauseStateChanged 载荷
{
    public PauseGroup Group { get; }
    public bool IsPaused { get; }
}
```

### 接口

```csharp
public interface IPauseHandler
{
    int Priority { get; }                              // 通知顺序，小优先
    void OnPauseStateChanged(PauseGroup group, bool isPaused);
}

public interface IPauseStackManager : IContextUtility
{
    PauseToken Push(string reason, PauseGroup group = PauseGroup.Global);
    bool Pop(PauseToken token);                        // 凭令牌弹出（任意顺序）
    bool IsPaused(PauseGroup group = PauseGroup.Global);
    int GetPauseDepth(PauseGroup group = PauseGroup.Global);
    IReadOnlyList<string> GetPauseReasons(PauseGroup group = PauseGroup.Global);  // 诊断
    IDisposable PauseScope(string reason, PauseGroup group = PauseGroup.Global);  // using 语法
    void ClearGroup(PauseGroup group);
    void ClearAll();
    void RegisterHandler(IPauseHandler handler);
    void UnregisterHandler(IPauseHandler handler);
    event EventHandler<PauseStateChangedEventArgs>? OnPauseStateChanged;
}
```

## 二、实现（PauseStackManager，610 行）

| 要点 | 行为 |
|---|---|
| 数据结构 | `Dictionary<PauseGroup, Stack<PauseEntry>>` + `Dictionary<Guid, PauseEntry>`（token 索引） |
| Push | 入栈 + 记录 token；**组从"未暂停→暂停"边界才通知**（锁外通知防死锁） |
| Pop | 凭 token 查索引；**支持移除栈中任意元素**（临时转移再恢复顺序）；仅"最后一人退出"才发恢复通知 |
| 并发 | `ReaderWriterLockSlim`；读多写少；通知在锁外快照派发 |
| 处理器 | `RegisterHandler` 注册；通知按 `Priority` 排序快照执行；单 handler 异常被捕获隔离（不中断他人） |
| 销毁 | 销毁时向仍暂停的组**补发恢复通知**，防止泄漏卡死 |
| 线程模型 | 单输入线程设计，跨线程需外层同步 |

### 支持类型

- `PauseEntry`（internal）：TokenId/Reason/Group/Timestamp
- `PauseScope`：`using (manager.PauseScope("reason", group)) { ... }` 自动 Push/Pop，防漏

## 三、Godot 桥接（关键认知）

`GodotPauseHandler`（Godot/Pause）：

```csharp
public class GodotPauseHandler : IPauseHandler
{
    public int Priority => 0;
    public void OnPauseStateChanged(PauseGroup group, bool isPaused)
    {
        if (group == PauseGroup.Global)      // ⚠️ 只监听 Global 组
        {
            _tree.Paused = isPaused;         // 接到 Godot SceneTree.Paused
        }
    }
}
```

**框架刻意只把 `Global` 组接到 Godot `SceneTree.Paused`。** Gameplay/Animation/Audio/Custom 组**不会**自动影响 Godot，需要项目自己：
- 注册自定义 `IPauseHandler` 响应组变化 → 控制节点 `ProcessMode`、音频总线、计时器等
- Godot 节点侧约定：想"暂停时仍动"的节点设 `ProcessMode.WhenPaused`，想"暂停时冻结"的保持默认

## 四、UI 联动（InteractionProfile）

`UiRouterBase` 已内置暂停集成（`SyncPauseRequest`）：

```csharp
// 页面 InteractionProfile 声明
public UiInteractionProfile InteractionProfile => new()
{
    PauseMode = UiPauseMode.WhileVisible,   // 页面可见期间持有暂停请求
    PauseGroup = PauseGroup.Global,          // 缺省 Global；可换 Gameplay 组（菜单不冻结 UI 但冻结游戏）
    PauseReason = "UI:OptionsMenu"           // 诊断可读
};
```

行为：页面 `OnShow`/入栈可见 → `Push(reason, group)`；`OnHide`/`OnExit`/销毁 → `Pop`。`UiPauseMode` 仅 `None`/`WhileVisible` 两态。前置条件：路由能拿到 `IPauseStackManager`（`TryBindPauseStackManager` 在 Utility 容器查，查不到则暂停联动静默禁用）。

**这套 = 游戏内菜单/弹窗自动暂停游戏的标准姿势**（弹设置/背包 → Global 暂停；弹对话/结算 → 只停 Gameplay 组）。

## 五、模板接线指南（gap → 步骤）

| 步骤 | 内容 |
|---|---|
| 1 | `UtilityModule` 注册 `PauseStackManager`（`RegisterUtility<IPauseStackManager>`） |
| 2 | 注册 `GodotPauseHandler(GetTree())` 到该 manager（Global 组 ↔ SceneTree.Paused 桥） |
| 3 | 自定义组监听：注册 Gameplay/Animation 组 handler（控制 ProcessMode/计时器） |
| 4 | Modal 类 UI 页面 InteractionProfile 设 `WhileVisible` + 目标组 |
| 5 | （模板既有）`GameInputController` 的 Paused 阶段输入依赖 `Tree.Paused`——接线后由框架驱动 |

## 六、测试注意

- `PauseStackManager` 是 `IContextUtility`——测试需先建架构上下文或直接 new 测纯逻辑
- 语义断言：嵌套 Push 不通知、最后一 Pop 才通知、非栈顶 Pop 保序、ClearGroup/ClearAll、destroy 补发
- 框架自带 `GFramework.Core.Tests` 覆盖此类；模板沉淀组件时参考

## 相关

- 扩展总览：`docs/research/gframework-extensions/README.md`
- 输入域（InteractionProfile 的另一半）：`docs/research/gframework-extensions/input-domain.md`
- 模板组件候选：countdown_timer（Animation/Gameplay 组暂停 vs 全局暂停的关系）
