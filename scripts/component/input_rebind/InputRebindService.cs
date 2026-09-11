using System;
using System.Collections.Generic;
using System.Linq;
using GFramework.Core.Abstractions.Utility;
using GFramework.Game.Abstractions.Input;
using Godot;

namespace GFrameworkTemplate.scripts.component.input_rebind;

/// <summary>
///     输入改键服务：捕获玩家输入、写入绑定、检测冲突并持久化。
/// </summary>
/// <remarks>
///     底层复用框架 <see cref="IInputBindingStore" />（改动会同步回 Godot InputMap），
///     本组件补齐框架缺失的<b>输入捕获</b>与<b>存档持久化</b>。
///     <para>典型流程与用法见 scripts/component/input_rebind/README.md。</para>
/// </remarks>
public sealed class InputRebindService : IUtility
{
    /// <summary>
    ///     手柄轴捕获阈值：绝对值需超过该值才认为玩家在推摇杆（过滤静止漂移）。
    /// </summary>
    private const float AxisCaptureThreshold = 0.5f;

    private readonly IInputBindingStore _store;
    private readonly List<string> _rebindableActions;
    private string? _capturingAction;
    private InputDeviceKind _capturingDeviceKind = InputDeviceKind.Unknown;

    /// <summary>
    ///     构造改键服务。
    /// </summary>
    /// <param name="store">底层绑定存储（通常为框架的 GodotInputBindingStore）。</param>
    /// <param name="rebindableActions">
    ///     参与改键的动作名集合；传 null 时自动取 InputMap 中所有非 <c>ui_</c> 前缀的动作。
    /// </param>
    public InputRebindService(IInputBindingStore store, IEnumerable<string>? rebindableActions = null)
    {
        ArgumentNullException.ThrowIfNull(store);

        _store = store;
        _rebindableActions = rebindableActions?.Distinct().ToList() ?? CollectDefaultActions();
    }

    /// <summary>
    ///     改键完成通知：参数为（动作名, 新绑定, 被交换的动作名或 null）。
    /// </summary>
    public event Action<string, InputBindingDescriptor, string?>? RebindCompleted;

    /// <summary>
    ///     捕获被取消的通知。
    /// </summary>
    public event Action? CaptureCancelled;

    /// <summary>
    ///     参与改键的动作名列表。
    /// </summary>
    public IReadOnlyList<string> RebindableActions => _rebindableActions;

    /// <summary>
    ///     当前是否处于捕获状态。
    /// </summary>
    public bool IsCapturing => _capturingAction is not null;

    /// <summary>
    ///     正在改键的动作名；未在捕获时为 null。
    /// </summary>
    public string? CapturingAction => _capturingAction;

    /// <summary>
    ///     本次捕获限定的设备类别（键鼠或手柄）。
    /// </summary>
    public InputDeviceKind CapturingDeviceKind => _capturingDeviceKind;

    /// <summary>
    ///     进入捕获状态，等待玩家输入。
    /// </summary>
    /// <param name="actionName">目标动作名。</param>
    /// <param name="deviceKind">限定捕获的设备类别（<see cref="InputDeviceKind.KeyboardMouse" /> 或 Gamepad）。</param>
    /// <exception cref="ArgumentException">动作名不在可改键列表，或设备类别不支持改键。</exception>
    public void BeginCapture(string actionName, InputDeviceKind deviceKind)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(actionName);

        if (!_rebindableActions.Contains(actionName))
        {
            throw new ArgumentException($"动作 {actionName} 不在可改键列表中。", nameof(actionName));
        }

        if (deviceKind is not (InputDeviceKind.KeyboardMouse or InputDeviceKind.Gamepad))
        {
            throw new ArgumentException($"设备类别 {deviceKind} 不支持改键。", nameof(deviceKind));
        }

        _capturingAction = actionName;
        _capturingDeviceKind = deviceKind;
    }

    /// <summary>
    ///     取消当前捕获。
    /// </summary>
    public void CancelCapture()
    {
        if (_capturingAction is null)
        {
            return;
        }

        _capturingAction = null;
        _capturingDeviceKind = InputDeviceKind.Unknown;
        CaptureCancelled?.Invoke();
    }

    /// <summary>
    ///     尝试用输入事件完成改键（非捕获状态或事件不匹配时返回 false）。
    /// </summary>
    /// <param name="inputEvent">输入事件。</param>
    /// <returns>true 表示已完成一次改键。</returns>
    public bool TryCapture(InputEvent inputEvent)
    {
        ArgumentNullException.ThrowIfNull(inputEvent);

        if (_capturingAction is null)
        {
            return false;
        }

        var binding = ToBinding(inputEvent);
        if (binding is null || !InputBindingDescriptorFactory.MatchesDevice(binding, _capturingDeviceKind))
        {
            return false;
        }

        var actionName = _capturingAction;
        var conflict = FindConflict(binding.Code, actionName);

        _capturingAction = null;
        _capturingDeviceKind = InputDeviceKind.Unknown;

        // swapIfTaken: 冲突时框架会把被占动作改绑为本动作此前的绑定（双向交换）
        _store.SetPrimaryBinding(actionName, binding, swapIfTaken: true);
        RebindCompleted?.Invoke(actionName, binding, conflict);

        return true;
    }

    /// <summary>
    ///     获取指定动作在某类设备下的全部绑定。
    /// </summary>
    /// <param name="actionName">动作名。</param>
    /// <param name="deviceKind">设备类别。</param>
    /// <returns>绑定列表（可能为空）。</returns>
    public IReadOnlyList<InputBindingDescriptor> GetBindings(string actionName, InputDeviceKind deviceKind)
    {
        return _store.GetBindings(actionName).Bindings
            .Where(binding => binding.DeviceKind == deviceKind)
            .ToList();
    }

    /// <summary>
    ///     获取指定动作在某类设备下的主绑定（改键后提供显示文本）。
    /// </summary>
    /// <param name="actionName">动作名。</param>
    /// <param name="deviceKind">设备类别。</param>
    /// <returns>主绑定；无绑定时返回 null。</returns>
    public InputBindingDescriptor? GetPrimaryBinding(string actionName, InputDeviceKind deviceKind)
    {
        return GetBindings(actionName, deviceKind).FirstOrDefault();
    }

    /// <summary>
    ///     查找已经占用该绑定的其他动作（用于改键前提示或改键后说明交换结果）。
    /// </summary>
    /// <param name="code">绑定 Code。</param>
    /// <param name="excludeAction">排除的动作名（通常为当前正在改键的动作）。</param>
    /// <returns>占用该绑定的动作名；无冲突时返回 null。</returns>
    public string? FindConflict(string code, string? excludeAction = null)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return null;
        }

        foreach (var action in _rebindableActions)
        {
            if (string.Equals(action, excludeAction, StringComparison.Ordinal))
            {
                continue;
            }

            if (_store.GetBindings(action).Bindings.Any(binding => binding.Code == code))
            {
                return action;
            }
        }

        return null;
    }

    /// <summary>
    ///     恢复单个动作的默认绑定。
    /// </summary>
    /// <param name="actionName">动作名。</param>
    public void ResetAction(string actionName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(actionName);
        _store.ResetAction(actionName);
    }

    /// <summary>
    ///     恢复全部动作的默认绑定。
    /// </summary>
    public void ResetAll()
    {
        _store.ResetAll();
    }

    /// <summary>
    ///     导出当前绑定快照（可直接交给框架恢复，也可交给本组件的存档方法落盘）。
    /// </summary>
    /// <returns>绑定快照。</returns>
    public InputBindingSnapshot ExportSnapshot()
    {
        return _store.ExportSnapshot();
    }

    /// <summary>
    ///     导入绑定快照（整体替换当前绑定）。
    /// </summary>
    /// <param name="snapshot">绑定快照。</param>
    public void ImportSnapshot(InputBindingSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        _store.ImportSnapshot(snapshot);
    }

    /// <summary>
    ///     把当前绑定保存到文件（JSON）。
    /// </summary>
    /// <param name="resPath">目标路径（如 <c>user://input_bindings.json</c>）。</param>
    /// <returns>保存成功返回 true。</returns>
    public bool SaveTo(string resPath)
    {
        return InputBindingSnapshotFile.Save(resPath, ExportSnapshot());
    }

    /// <summary>
    ///     从文件加载绑定并应用。
    /// </summary>
    /// <param name="resPath">来源路径。</param>
    /// <returns>加载并应用成功返回 true；文件缺失或内容非法返回 false。</returns>
    public bool LoadFrom(string resPath)
    {
        var snapshot = InputBindingSnapshotFile.Load(resPath);
        if (snapshot is null)
        {
            return false;
        }

        ImportSnapshot(snapshot);
        return true;
    }

    /// <summary>
    ///     从 InputMap 收集默认的可改键动作（排除引擎内置的 <c>ui_</c> 动作）。
    /// </summary>
    /// <returns>动作名列表。</returns>
    private static List<string> CollectDefaultActions()
    {
        return InputMap.GetActions()
            .Select(static action => action.ToString())
            .Where(static name => !name.StartsWith("ui_", StringComparison.Ordinal))
            .OrderBy(static name => name, StringComparer.Ordinal)
            .ToList();
    }

    /// <summary>
    ///     把 Godot 输入事件转成绑定描述（仅"按下"类事件，轴需超过捕获阈值）。
    /// </summary>
    /// <param name="inputEvent">输入事件。</param>
    /// <returns>绑定描述；无法转换时返回 null。</returns>
    private static InputBindingDescriptor? ToBinding(InputEvent inputEvent)
    {
        return inputEvent switch
        {
            // 排除按键回显（长按重复）事件
            InputEventKey { Pressed: true, Echo: false } key =>
                InputBindingDescriptorFactory.ForKey((int)key.PhysicalKeycode, OS.GetKeycodeString(key.PhysicalKeycode)),
            InputEventMouseButton { Pressed: true } mouse =>
                InputBindingDescriptorFactory.ForMouse((int)mouse.ButtonIndex),
            InputEventJoypadButton { Pressed: true } button =>
                InputBindingDescriptorFactory.ForGamepadButton((int)button.ButtonIndex),
            // 轴需超过阈值：避免摇杆静止漂移被误判为改键输入
            InputEventJoypadMotion motion when Mathf.Abs(motion.AxisValue) >= AxisCaptureThreshold =>
                InputBindingDescriptorFactory.ForGamepadAxis((int)motion.Axis, motion.AxisValue),
            _ => null,
        };
    }
}
