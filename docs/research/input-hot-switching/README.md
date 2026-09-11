# 输入热切换研究（设备识别 / 提示图标 / 焦点归位）

> 研究目标：让游戏在**键鼠 ↔ 手柄 ↔ 触摸**之间热切换时，自动切换按键提示图标、焦点环与交互模式，
> 并覆盖新型手柄（本次以 Steam Controller 2 代实测）。
>
> 状态：设备识别已沉淀为模板组件（`InputDeviceService`）；改键能力见 [input-rebind 教程](../../guides/input-rebind.md)；
> 提示图标与焦点归位属表现层做法，按第七章在项目内实现。

## 一、研究结论速览

| 问题 | 结论 |
|---|---|
| Godot 在 Windows 上的手柄后端 | **4.5+ 起使用 SDL 3**（4.5 之前为引擎自研代码）；上限 4 只手柄 |
| Godot 是否有 HID 原始访问 API | **没有**（官方提案 [#12089](https://github.com/godotengine/godot-proposals/issues/12089) 未实现） |
| 是否有全局摇杆死区设置 | **没有**；死区仅存在于 **InputMap 动作级**（每个动作自带 `deadzone`） |
| 默认 `ui_accept` 能用手柄确认吗 | **不能**：默认仅 Enter / KP Enter / Space，需显式补手柄 A（见下文「坑位」） |
| `joy.cpl` 看不到手柄 | **属正常**：它只列 DirectInput 老式设备，XInput / 虚拟手柄不会出现 |
| Steam Controller 2 能否被 Godot 识别 | **可以**，但需先经 Steam 转成虚拟手柄（见下）；且**只有 XInput 能力子集** |

## 二、Godot 手柄输入的三个层次（排查顺序）

遇到"手柄没反应"时按此顺序定位，避免误判：

```
① 硬件 / 系统层   → 浏览器 https://hardwaretester.com/gamepad（Gamepad API，比 joy.cpl 可靠）
② 引擎层          → Input.GetConnectedJoypads() / GetJoyName(id) / GetJoyGuid(id)
③ 应用层          → 引擎内打印事件类型统计与摇杆实时值（见下方建议）
```

模板未内置调试探针（避免长期占用 UI 入口）。排查时建议临时加一个 Label 或日志，打印：
`Input.GetConnectedJoypads()` / `GetJoyName` / `GetJoyGuid`、`Input.GetJoyAxis()` 实时值、
以及收到的 `InputEvent` 类型（区分 `InputEventJoypadButton` / `JoypadMotion` / `Key` / `MouseMotion`）。
这套"事件类型统计 + 摇杆实时值"在定位手柄问题时最有效。

## 三、坑位：手柄能导航却无法确认按钮

**现象**：手柄接管后 UI 焦点能随摇杆/方向键移动，但按 A 无法激活按钮；键鼠一切正常。

**根因**（已用 SceneTree 脚本实测 Godot 默认 InputMap）：

| 内置动作 | 默认绑定 | 手柄可用？ |
|---|---|---|
| `ui_left` / `ui_right` / `ui_up` / `ui_down` | 方向键 + 手柄 DPad(11–14) + 左摇杆轴 | ✅ 导航正常 |
| `ui_accept` | Enter、KP Enter、Space | ❌ **无手柄 A** |
| `ui_cancel` | Escape | ❌ **无手柄 B** |

即：**引擎默认只给导航动作配了手柄，确认/取消没配**。这不是配置写错，而是默认值缺口。

**修复**：在 `project.godot` 显式定义这两个动作。注意**显式定义会整体覆盖默认值**，
因此必须同时保留原有键盘绑定：

```ini
ui_accept={ "deadzone": 0.5, "events": [Enter, KP Enter, Space, 手柄 A(0)] }
ui_cancel={ "deadzone": 0.5, "events": [Escape, 手柄 B(1)] }
```

**验证方法**（无需改代码，临时脚本即可）：

```gdscript
extends SceneTree
func _init():
    for e in InputMap.action_get_events("ui_accept"):
        if e is InputEventJoypadButton:
            print("joybtn idx=%d device=%d" % [e.button_index, e.device])
    quit()
```

> `device = -1` 表示「所有手柄」；若为具体索引则只对那只手柄生效。

## 四、实测：Steam Controller 2 代在 Godot 下的表现

> 环境：Windows + Godot 4.7.2 (.NET) + Steam 运行中 + Steam Controller 2 代
> 前置条件：Steam 桌面布局需设为「手柄」模板；若引擎仍收不到输入，见下文 issue #119465 的绕过方式。

| SC2 物理输入 | Godot 收到的事件 | 说明 |
|---|---|---|
| A / B / X / Y | `InputEventJoypadButton`（标准按钮） | 正常 |
| 方向键 | `InputEventJoypadButton`（DPad*） | 正常 |
| 摇杆 | `InputEventJoypadMotion`（LeftX/LeftY…） | 静止时有微小噪声，需死区过滤 |
| **右触控板按下** | `InputEventJoypadButton`（**右摇杆按下** RS） | 被 Steam 映射为右摇杆点击 |
| **右触控板滑动** | `InputEventJoypadMotion`（**右摇杆轴**） | 被 Steam 映射为右摇杆移动 |
| **背键（L4/L5/R4/R5）** | **无事件** | XInput 无背键概念，Steam 未映射 |
| **陀螺仪** | **无事件** | XInput 无陀螺仪通道 |

**关键结论**：Godot 当前收到的是 **Steam 转换后的虚拟 XInput 手柄**，因此：
- 触控板、背键、陀螺仪等 SC2 特有能力**被降级/丢弃**（除非在 Steam 端手工映射到标准按钮）
- 游戏侧只能看到标准 XInput 按钮与轴

## 五、已知引擎问题：Steam Controller 在非 Steam 启动时收不到输入

**godotengine/godot#119465**（Steam Controller 2026 + 桌面布局设为 Gamepad）：

- 现象：Godot 收不到该手柄输入，但 Gamepad Tester、KDE 手柄测试器、Godot Web Editor 都能收到
- **绕过方式（issue 内确认有效）**：
  > "The gamepad does however work if I add Godot to Steam and launch it from there."
  即：**把 Godot 编辑器加入 Steam 库，从 Steam 启动**，再打开项目

因此排查 SC2 在 Godot 中的接入问题时，"从 Steam 启动 Godot"是必要验证步骤之一。

## 六、要拿到手柄全部能力，只有一条官方路径

| 路径 | 可行性 | 说明 |
|---|---|---|
| Godot 原生 HID 直读 | ❌ | 引擎无 HID API；设备协议为厂商专有（社区只能逐设备写 GDExtension） |
| 虚拟 XInput 手柄（当前方案） | ✅ 但降级 | 只保留标准按钮/轴，触控板/背键/陀螺仪丢失 |
| **Steamworks Steam Input API** | ✅ 完整 | 经 GodotSteam 的 Steam Input 集成；动作映射式 API，天然屏蔽设备差异 |

Steam Input API 的两个额外价值（对"热切换"主题直接相关）：

1. **官方按键图标 API**：`GetGlyphForActionOrigin` / `GetGlyphPNGForActionOrigin`
   —— 由 Steam 按**当前实际设备**返回对应图标（Xbox/PS/Switch/Steam 手柄各异），
   无需自己维护"设备 → 图标集"映射表
2. **设备接入/断开回调**：`input_device_connected` / `input_device_disconnected`
   —— 热插拔与设备切换的原生事件源

代价：需要引入 Steamworks SDK（GodotSteam）+ In-Game Actions 文件，且发布渠道为 Steam。
**属项目级决策，不宜内置到通用模板。**

## 七、热切换的三块拼图

| 拼图 | 状态 | 说明 |
|---|---|---|
| ① 设备识别（活跃设备 + 变化通知） | ✅ 已沉淀为组件 | `scripts/framework/input/InputDeviceService.cs`：包框架 `IInputDeviceTracker`，暴露 `DeviceChanged` / `IsGamepad` / `IsKeyboardMouse` / `IsTouch`；`GameInputController` 在每个输入事件上喂入 |
| ② 提示图标切换（按键提示随设备变化） | 📘 表现层做法 | 订阅 `DeviceChanged` 替换提示文本或图标（见下） |
| ③ 焦点归位（手柄接管时聚焦默认控件） | 📘 表现层做法 | 手柄接管且菜单可见时 `GrabFocus()`，切回键鼠 `ReleaseFocus()`（见下） |

模板只沉淀 ①（与业务无关、跨项目可复用）；②③ 依赖各项目的 UI 结构，按下述做法在项目内实现。

### ② 提示图标切换

```csharp
deviceService.DeviceChanged += context =>
{
    var useGamepad = context.DeviceKind == InputDeviceKind.Gamepad;
    hintLabel.Text = useGamepad ? "左摇杆 / A 键" : "W A S D / E";
};
```

若走 Steam Input API，可直接用官方 glyph API 按当前设备取图标（见第六节），无需自维护映射表。

### ③ 焦点归位

```csharp
// 手柄接管且菜单打开 → 焦点交给默认控件；切回键鼠 → 释放，避免残留焦点环
private void OnDeviceChanged(InputDeviceContext context)
{
    if (!_menuVisible) return;

    if (context.DeviceKind == InputDeviceKind.Gamepad) _defaultButton.GrabFocus();
    else _defaultButton.ReleaseFocus();
}
```

两个配套要点：

- 菜单**打开时**若当前就是手柄，也要直接 `GrabFocus()`，否则玩家需先动摇杆才会出现焦点
- Godot 内置 `ui_*` 动作（`ui_accept`/`ui_cancel`/`ui_left`…）+ `Control.FocusMode` 已提供手柄导航，
  无需自研导航逻辑；注意 `ui_accept`/`ui_cancel` 默认不含手柄按钮，需按第三节补齐

关键的工程约束：**游戏逻辑不应感知设备**。移动/交互只读 InputMap 动作（如 `Input.GetVector("move_left", ...)`），
设备切换只影响提示与焦点——这样新增设备或改键都不需要改动玩法代码。

## 八、参考

- Godot 文档：[Controllers, gamepads, and joysticks](https://docs.godotengine.org/en/stable/tutorials/inputs/controllers_gamepads_joysticks.html)
- Godot issue：[#119465 — Steam Controller (2026) 在非 Steam 启动时收不到输入](https://github.com/godotengine/godot/issues/119465)
- Godot 提案：[#12089 — 提供 HID 输入/输出报告 API](https://github.com/godotengine/godot-proposals/issues/12089)
- Godot 提案：[#3709 — 可配置的摇杆轴死区](https://github.com/godotengine/godot-proposals/issues/3709)
- GodotSteam：[Steam Input 集成教程](https://godotsteam.com/tutorials/inputs/)
- 框架输入域研究：[../gframework-extensions/input-domain.md](../gframework-extensions/input-domain.md)
