# 输入改键组件（input_rebind）

让玩家把**键鼠与手柄的按键绑定改成自己习惯的**：捕获输入 → 写入绑定 → 冲突自动交换 → 存档持久化。

## 为什么需要它

框架已经提供了绑定存储（`IInputBindingStore`）：读绑定、写绑定、冲突交换、恢复默认、快照导入导出都现成。
但它缺少两块，本项目改键功能卡在这里：

| 缺口 | 说明 | 组件补齐 |
|---|---|---|
| **输入捕获** | 框架内部的 `GodotInputBindingCodec`（`InputEvent` → 绑定描述）是 `internal`，外部拿不到 | `InputRebindService.TryCapture` + `InputBindingDescriptorFactory` |
| **存档持久化** | 框架快照类型只有只读属性，不能直接序列化 | `InputBindingSnapshotFile`（DTO + JSON） |

## 核心概念

```
开始捕获 (动作, 设备类别)
   ↓ 玩家按下按键/推摇杆
输入事件 → 绑定描述（key:65 / joy-button:0 / joy-axis:0:-1）
   ↓ 冲突检测（别的动作已占用这个键？）
SetPrimaryBinding(swapIfTaken: true)   ← 框架自动把双方绑定互换
   ↓
RebindCompleted 事件（含被交换的动作名） → UI 刷新 → SaveTo(user://…)
```

**绑定 Code 格式**（与框架保持一致，存档兼容）：

| 设备 | 格式 | 示例 |
|---|---|---|
| 键盘 | `key:{Godot 键码}` | `key:87`（W） |
| 鼠标 | `mouse:{按钮索引}` | `mouse:1`（左键） |
| 手柄按键 | `joy-button:{按钮索引}` | `joy-button:0`（A） |
| 手柄轴 | `joy-axis:{轴索引}:{±1}` | `joy-axis:0:-1`（左摇杆向左） |

## 快速上手

```csharp
// 1. 从架构取服务（UtilityModule 已注册；也可自行 new）
var rebind = this.GetUtility<InputRebindService>();

// 2. UI 点「改键」→ 进入捕获态（限定本次要改的设备类别）
rebind.BeginCapture("move_up", InputDeviceKind.KeyboardMouse);

// 3. 把输入事件喂进去（放在 _Input 或输入控制器里）
public override void _Input(InputEvent @event)
{
    if (rebind.TryCapture(@event))
    {
        GetViewport().SetInputAsHandled();   // 捕获成功即消费掉，避免触发游戏动作
    }
}

// 4. 监听结果刷新界面
rebind.RebindCompleted += (action, binding, swappedWith) =>
{
    _label.Text = binding.DisplayName;                       // 如 "W"
    if (swappedWith is not null)
        _toast.Text = $"已与「{swappedWith}」交换绑定";        // 冲突提示
};

// 5. 读当前绑定用于显示
var current = rebind.GetPrimaryBinding("move_up", InputDeviceKind.KeyboardMouse);

// 6. 恢复默认 / 持久化
rebind.ResetAction("move_up");
rebind.ResetAll();
rebind.SaveTo("user://input_bindings.json");
rebind.LoadFrom("user://input_bindings.json");   // 启动时调用即可
```

## API 速查

| 成员 | 用途 |
|---|---|
| `RebindableActions` | 可改键动作列表（构造时传入，或自动取 InputMap 中非 `ui_` 动作） |
| `BeginCapture(action, deviceKind)` | 进入捕获态（设备类别限定了只收键鼠还是手柄输入） |
| `TryCapture(inputEvent)` | 尝试用事件完成改键；返回 true 表示已完成 |
| `CancelCapture()` / `IsCapturing` / `CapturingAction` | 取消捕获与状态查询 |
| `GetBindings(action, deviceKind)` / `GetPrimaryBinding(...)` | 读绑定（按设备筛选） |
| `FindConflict(code, excludeAction)` | 查某绑定是否已被其他动作占用 |
| `ResetAction` / `ResetAll` | 恢复默认 |
| `ExportSnapshot` / `ImportSnapshot` | 与框架快照互换 |
| `SaveTo` / `LoadFrom` | JSON 存档读写 |
| 事件 `RebindCompleted` / `CaptureCancelled` | 改键完成（含交换对象）/ 捕获取消 |

**工厂**（`InputBindingDescriptorFactory`）：`ForKey` / `ForMouse` / `ForGamepadButton` / `ForGamepadAxis` / `MatchesDevice`
—— 只接受基本类型，因此可脱离 Godot 运行时单测。

## 坑位

| 坑 | 说明 |
|---|---|
| 轴捕获需阈值 | 摇杆静止有微小漂移，组件已用 `0.5` 阈值过滤（低于阈值不作为改键输入） |
| 按键回显 | 长按会重复产生 `input_event`，组件已排除 `Echo` 事件 |
| 冲突不是错误 | 框架会**自动交换**双方绑定，UI 应提示"已与 X 交换"而非报错 |
| 显式覆盖 InputMap | 改键写回 Godot InputMap；`ui_*` 动作不适合交给玩家改（组件默认排除） |
| 捕获期要消费事件 | 捕获成功后应 `SetInputAsHandled()`，否则新键会同时触发游戏动作 |
| 存档路径 | 用 `user://`（导出后仍可写），不要写 `res://` |

## 测试

`tests/My-GFramework-Godot-Template.Tests/InputRebindTests.cs`（13 个用例，纯 .NET 可跑）：
Code 格式、设备/种类判定、显示名回退、冲突查找、主绑定读取、恢复默认、捕获状态机与参数校验、快照序列化往返、非法 JSON 容错、导入生效。

> 捕获 `InputEvent` 的部分依赖 Godot 运行时，需在引擎内验证（见 `docs/guides/input-rebind.md`）。

## 与设备热切换的关系

本组件与 `scripts/framework/input/InputDeviceService.cs`（设备识别/热切换）配套：
改键时用设备类别区分键鼠与手柄绑定，与运行时"按当前设备切换提示"使用的是同一套设备概念。
