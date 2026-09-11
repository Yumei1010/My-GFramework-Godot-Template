# 输入改键教程（Input Rebinding）

> **一句话**：框架已经能做改键（读写绑定、冲突交换、恢复默认、快照导入导出），
> 缺的是"捕获玩家按了哪个键"和"把快照存成文件"，本组件补齐这两块。
>
> 组件位置：`scripts/component/input_rebind/`｜验证测试：`tests/…/InputRebindTests.cs`（13 用例）

## 1. 为什么需要改键

- 玩家习惯不同：有人用 WASD，有人用方向键，有人要 ESDF
- 手柄布局不同：Xbox / PS / Switch / Steam Controller 的 A 键位置都不一样
- 无障碍需求：单手操作、特殊外设

自己实现会踩的坑：冲突交换、存档兼容、设备区分（键鼠一套、手柄一套）、摇杆漂移误捕获。
这些框架 + 本组件都已经处理。

## 2. 框架已有的能力（别造轮子）

`IInputBindingStore`（框架提供，写操作会同步回 Godot `InputMap`）：

```csharp
store.GetBindings(actionName);                                 // 读
store.SetPrimaryBinding(action, binding, swapIfTaken: true);   // 写（冲突自动交换）
store.ResetAction(actionName);  store.ResetAll();              // 恢复默认
store.ExportSnapshot();  store.ImportSnapshot(snapshot);       // 存档交换
```

**冲突交换的语义**：A 动作改成已被 B 占用的键 → A 拿到该键，B 改绑为 A 此前的键（双向互换，不丢绑定）。

## 3. 本组件补齐的两块

| 缺口 | 组件对应 |
|---|---|
| 框架 `GodotInputBindingCodec` 是 `internal`，拿不到 `InputEvent → 绑定描述` | `InputRebindService.TryCapture` + `InputBindingDescriptorFactory` |
| 框架快照类型只有只读属性，不能直接序列化 | `InputBindingSnapshotFile`（DTO + `System.Text.Json`） |

## 4. 完整接入步骤

### 步骤 1：注册服务（模板已做）

`scripts/module/UtilityModule.cs`：

```csharp
var inputDeviceService = new InputDeviceService();
architecture.RegisterUtility(inputDeviceService);

// 动作列表省略时自动取 InputMap 中所有非 ui_ 前缀的动作
architecture.RegisterUtility(new InputRebindService(inputDeviceService.BindingStore));
```

### 步骤 2：启动时加载存档

```csharp
const string BindingPath = "user://input_bindings.json";
this.GetUtility<InputRebindService>().LoadFrom(BindingPath);   // 无文件时安全返回 false
```

### 步骤 3：把输入喂给捕获

放在能拿到全部输入事件的地方（`_Input`，或自己的输入控制器里）：

```csharp
public override void _Input(InputEvent @event)
{
    // 非捕获状态直接返回，开销可忽略
    if (this.GetUtility<InputRebindService>().TryCapture(@event))
    {
        GetViewport().SetInputAsHandled();   // 关键：消费掉，别让新键同时触发游戏动作
    }
}
```

### 步骤 4：做改键 UI（界面骨架）

```csharp
// 每行一个动作：显示当前绑定 + 改键按钮
var rebind = this.GetUtility<InputRebindService>();

foreach (var action in rebind.RebindableActions)
{
    var current = rebind.GetPrimaryBinding(action, InputDeviceKind.KeyboardMouse);
    AddRow(action, current?.DisplayName ?? "未绑定", onRebind: () =>
    {
        rebind.BeginCapture(action, InputDeviceKind.KeyboardMouse);
        button.Text = "请按键…";
    });
}

rebind.RebindCompleted += (action, binding, swappedWith) =>
{
    RefreshRow(action, binding.DisplayName);
    if (swappedWith is not null)
        ShowToast($"已与「{swappedWith}」交换绑定");
    rebind.SaveTo(BindingPath);          // 改完即存
};

rebind.CaptureCancelled += () => RefreshAll();
```

### 步骤 5：键鼠 / 手柄分开处理

同一个动作在两套设备上各有一条绑定，改键时用设备类别区分：

```csharp
rebind.BeginCapture("move_up", InputDeviceKind.KeyboardMouse);   // 只等键盘/鼠标输入
rebind.BeginCapture("move_up", InputDeviceKind.Gamepad);         // 只等手柄输入
```

捕获期间**不匹配设备的事件会被忽略**，所以玩家按到别的设备不会误绑定。

## 5. 存档格式

`user://input_bindings.json`（缩进输出，便于排查；枚举以字符串存储，向后兼容）：

```json
{
  "Actions": {
    "move_up": [
      {
        "Device": "KeyboardMouse",
        "Kind": "Key",
        "Code": "key:87",
        "Name": "W",
        "AxisDirection": null
      }
    ]
  }
}
```

Code 格式与框架保持一致：`key:87` / `mouse:1` / `joy-button:0` / `joy-axis:0:-1`。

## 6. 常见坑

| 坑 | 现象 | 处理 |
|---|---|---|
| 捕获期未消费事件 | 改成哪个键，那个键立刻触发游戏动作 | 捕获成功后 `SetInputAsHandled()` |
| 摇杆漂移被当作改键 | 还没碰摇杆就绑定成功 | 组件已用 0.5 阈值过滤；阈值见 `AxisCaptureThreshold` |
| 长按重复触发 | 捕获时按住不放会重复处理 | 组件已排除 `Echo` 事件 |
| 把冲突当错误提示 | 玩家看到"按键已被占用"就不敢改 | 框架会自动交换，提示"已与 X 交换"更准确 |
| 让玩家改 `ui_*` 动作 | UI 导航键被改乱，菜单打不开 | 组件默认排除 `ui_` 前缀；建议只暴露业务动作 |
| 存档写 `res://` | 导出后只读，保存失败 | 用 `user://` |
| 改了键但游戏内没生效 | 绑定写在框架内存态，未同步 | 框架写操作会回写 InputMap；若自己缓存了绑定需重新读 |

## 7. API 速查

| 类别 | 成员 |
|---|---|
| 捕获 | `BeginCapture(action, deviceKind)` / `TryCapture(event)` / `CancelCapture()` / `IsCapturing` / `CapturingAction` / `CapturingDeviceKind` |
| 读取 | `RebindableActions` / `GetBindings(action, deviceKind)` / `GetPrimaryBinding(action, deviceKind)` |
| 冲突 | `FindConflict(code, excludeAction)` |
| 重置 | `ResetAction(action)` / `ResetAll()` |
| 存档 | `ExportSnapshot()` / `ImportSnapshot(snapshot)` / `SaveTo(path)` / `LoadFrom(path)` |
| 事件 | `RebindCompleted(action, binding, swappedWith)` / `CaptureCancelled` |
| 工厂 | `InputBindingDescriptorFactory.ForKey/ForMouse/ForGamepadButton/ForGamepadAxis/MatchesDevice` |

## 8. 框架源码阅读入口

| 内容 | 位置 |
|---|---|
| 绑定存储（纯逻辑，冲突交换算法） | `GFramework.Game/Input/InputBindingStore.cs` |
| Godot 适配（读写 InputMap，公开入口） | `GFramework.Godot/Input/GodotInputBindingStore.cs` |
| Code 格式转换（internal，本组件按其格式实现） | `GFramework.Godot/Input/GodotInputBindingCodec.cs` |
| 绑定值类型 | `GFramework.Game.Abstractions/Input/`（`InputBindingDescriptor` 等） |

## 9. 验证测试清单

`dotnet test --filter FullyQualifiedName~InputRebindTests`（13 个用例，纯 .NET 可跑）：

- 绑定工厂：各类绑定的 Code 格式、设备/种类判定、显示名回退、设备匹配
- 服务：冲突查找（含排除自身、无冲突）、按设备读主绑定、恢复默认、捕获状态机、参数校验
- 快照：序列化往返、轴方向保留、非法 JSON 容错、导入后可读

> 捕获 `InputEvent → 绑定描述` 依赖 Godot 运行时，单测无法覆盖，
> 需在引擎内验证：运行 `scenes/demo/rpg_demo.tscn`，按 F1 打开输入探针确认按键事件正常后，
> 在改键 UI 上实测（若尚未接入 UI，可用 `TryCapture` 的最小示例验证）。

## 10. 与输入热切换的关系

| 能力 | 位置 |
|---|---|
| 设备识别（键鼠/手柄/触摸切换 + 提示图标 + 焦点归位） | `scripts/framework/input/InputDeviceService.cs` + `docs/research/input-hot-switching/README.md` |
| 改键（本组件） | `scripts/component/input_rebind/` |

两者共用同一套设备概念（`InputDeviceKind`）：热切换决定"现在显示哪套提示"，改键决定"这套提示对应哪个键"。
