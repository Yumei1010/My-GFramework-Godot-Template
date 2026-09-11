using System;
using GFramework.Core.Abstractions.Utility;
using GFramework.Game.Abstractions.Input;
using GFramework.Godot.Input;
using Godot;

namespace GFrameworkTemplate.scripts.framework.input;

/// <summary>
///     输入设备服务：跟踪当前活跃设备（键鼠 / 手柄 / 触摸）并在变化时广播。
/// </summary>
/// <remarks>
///     基于框架 <see cref="GodotInputBindingStore" />，事件经 <see cref="Feed" /> 喂入；
///     设备类别或手柄索引变化时触发 <see cref="DeviceChanged" />。
///     <para>框架 tracker 不区分具体手柄，索引变化同样会通知。</para>
/// </remarks>
public sealed class InputDeviceService : IUtility
{
    private readonly GodotInputBindingStore _store = new();
    private InputDeviceKind _lastKind = InputDeviceKind.Unknown;
    private int? _lastIndex;

    /// <summary>
    ///     设备变化通知：参数为变化后的设备上下文。
    /// </summary>
    /// <remarks>
    ///     首次确定设备（从 Unknown 变为具体设备）时也会触发。
    /// </remarks>
    public event Action<InputDeviceContext>? DeviceChanged;

    /// <summary>
    ///     获取当前活跃设备上下文。
    /// </summary>
    public InputDeviceContext CurrentDevice => _store.CurrentDevice;

    /// <summary>
    ///     获取当前设备类别。
    /// </summary>
    public InputDeviceKind CurrentKind => _store.CurrentDevice.DeviceKind;

    /// <summary>
    ///     获取底层框架绑定存储（可做重绑定、快照导入导出等）。
    /// </summary>
    public GodotInputBindingStore BindingStore => _store;

    /// <summary>
    ///     是否当前使用手柄输入。
    /// </summary>
    public bool IsGamepad => CurrentKind == InputDeviceKind.Gamepad;

    /// <summary>
    ///     是否当前使用键鼠输入。
    /// </summary>
    public bool IsKeyboardMouse => CurrentKind == InputDeviceKind.KeyboardMouse;

    /// <summary>
    ///     是否当前使用触摸输入。
    /// </summary>
    public bool IsTouch => CurrentKind == InputDeviceKind.Touch;

    /// <summary>
    ///     喂入一个输入事件，更新当前活跃设备；设备变化时广播通知。
    /// </summary>
    /// <param name="inputEvent">Godot 输入事件。</param>
    public void Feed(InputEvent inputEvent)
    {
        ArgumentNullException.ThrowIfNull(inputEvent);

        _store.UpdateDeviceFromInput(inputEvent);

        var context = _store.CurrentDevice;
        if (context.DeviceKind == _lastKind && context.DeviceIndex == _lastIndex)
        {
            return;
        }

        _lastKind = context.DeviceKind;
        _lastIndex = context.DeviceIndex;
        DeviceChanged?.Invoke(context);
    }

    /// <summary>
    ///     主动设置设备（例如由 UI 预设或测试驱动），变化时同样广播通知。
    /// </summary>
    /// <param name="context">目标设备上下文。</param>
    public void SetDevice(InputDeviceContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        _lastKind = context.DeviceKind;
        _lastIndex = context.DeviceIndex;
        DeviceChanged?.Invoke(context);
    }

    /// <summary>
    ///     获取当前设备的中文显示名（用于调试/提示）。
    /// </summary>
    /// <returns>设备显示名。</returns>
    public string GetDeviceDisplayName()
    {
        var context = _store.CurrentDevice;
        return context.DeviceKind switch
        {
            InputDeviceKind.KeyboardMouse => "键盘 / 鼠标",
            InputDeviceKind.Gamepad => string.IsNullOrEmpty(context.DeviceName)
                ? $"手柄 #{context.DeviceIndex ?? 0}"
                : $"{context.DeviceName} #{context.DeviceIndex ?? 0}",
            InputDeviceKind.Touch => "触摸",
            _ => "未知",
        };
    }
}
