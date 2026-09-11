using System.Linq;
using GFramework.Game.Abstractions.Input;
using GFramework.Game.Input;
using GFrameworkTemplate.scripts.component.input_rebind;
using Xunit;

namespace GFrameworkTemplate.Tests;

/// <summary>
///     输入改键组件验证测试：绑定描述格式、快照序列化、冲突检测与重置。
/// </summary>
/// <remarks>
///     只覆盖不依赖 Godot 运行时的逻辑：绑定工厂接受基本类型，
///     快照序列化为纯 .NET 实现，服务可注入框架的纯逻辑 <see cref="InputBindingStore" />。
///     捕获 <c>InputEvent</c> 的行为需在引擎内验证（见 docs/guides/input-rebind.md）。
/// </remarks>
public class InputRebindTests
{
    /// <summary>
    ///     构造带默认绑定的改键服务（jump=Space、fire=J）。
    /// </summary>
    /// <param name="store">输出的底层绑定存储。</param>
    /// <returns>改键服务。</returns>
    private static InputRebindService CreateService(out InputBindingStore store)
    {
        var defaults = new InputBindingSnapshot(
        [
            new InputActionBinding("jump", [InputBindingDescriptorFactory.ForKey(32, "Space")]),
            new InputActionBinding("fire", [InputBindingDescriptorFactory.ForKey(74, "J")]),
        ]);

        store = new InputBindingStore(defaults);
        return new InputRebindService(store, ["jump", "fire"]);
    }

    [Fact]
    public void 绑定工厂_各类绑定的Code格式符合框架约定()
    {
        Assert.Equal("key:65", InputBindingDescriptorFactory.ForKey(65, "A").Code);
        Assert.Equal("mouse:1", InputBindingDescriptorFactory.ForMouse(1).Code);
        Assert.Equal("joy-button:0", InputBindingDescriptorFactory.ForGamepadButton(0).Code);
        Assert.Equal("joy-axis:1:-1", InputBindingDescriptorFactory.ForGamepadAxis(1, -0.8f).Code);
        Assert.Equal("joy-axis:1:1", InputBindingDescriptorFactory.ForGamepadAxis(1, 0.8f).Code);
    }

    [Fact]
    public void 绑定工厂_设备类别与绑定种类正确()
    {
        var key = InputBindingDescriptorFactory.ForKey(65);
        Assert.Equal(InputDeviceKind.KeyboardMouse, key.DeviceKind);
        Assert.Equal(InputBindingKind.Key, key.BindingKind);

        var axis = InputBindingDescriptorFactory.ForGamepadAxis(0, -1f);
        Assert.Equal(InputDeviceKind.Gamepad, axis.DeviceKind);
        Assert.Equal(InputBindingKind.GamepadAxis, axis.BindingKind);
        Assert.Equal(-1f, axis.AxisDirection);
    }

    [Fact]
    public void 绑定工厂_未提供显示名时退化为可读占位文本()
    {
        Assert.Equal("A", InputBindingDescriptorFactory.ForKey(65, "A").DisplayName);
        Assert.Equal("Key 65", InputBindingDescriptorFactory.ForKey(65).DisplayName);
        Assert.Equal("Button 0", InputBindingDescriptorFactory.ForGamepadButton(0).DisplayName);
    }

    [Fact]
    public void 绑定工厂_设备匹配判定()
    {
        Assert.True(InputBindingDescriptorFactory.MatchesDevice(
            InputBindingDescriptorFactory.ForKey(65), InputDeviceKind.KeyboardMouse));
        Assert.False(InputBindingDescriptorFactory.MatchesDevice(
            InputBindingDescriptorFactory.ForKey(65), InputDeviceKind.Gamepad));
    }

    [Fact]
    public void 服务_查找冲突绑定返回占用它的动作()
    {
        var service = CreateService(out _);

        Assert.Equal("jump", service.FindConflict("key:32", "fire"));
        Assert.Equal("fire", service.FindConflict("key:74", "jump"));
        // 排除自身后不应报告冲突
        Assert.Null(service.FindConflict("key:32", "jump"));
        // 未被占用的绑定无冲突
        Assert.Null(service.FindConflict("key:999"));
    }

    [Fact]
    public void 服务_读取指定设备的主绑定()
    {
        var service = CreateService(out _);

        var binding = service.GetPrimaryBinding("jump", InputDeviceKind.KeyboardMouse);

        Assert.NotNull(binding);
        Assert.Equal("key:32", binding!.Code);
        Assert.Equal("Space", binding.DisplayName);
        // 该动作没有手柄绑定
        Assert.Null(service.GetPrimaryBinding("jump", InputDeviceKind.Gamepad));
    }

    [Fact]
    public void 服务_恢复到默认绑定()
    {
        var service = CreateService(out var store);

        store.SetPrimaryBinding("jump", InputBindingDescriptorFactory.ForKey(75, "K"));
        Assert.Equal("key:75", service.GetPrimaryBinding("jump", InputDeviceKind.KeyboardMouse)!.Code);

        service.ResetAction("jump");
        Assert.Equal("key:32", service.GetPrimaryBinding("jump", InputDeviceKind.KeyboardMouse)!.Code);
    }

    [Fact]
    public void 服务_捕获状态可开始与取消()
    {
        var service = CreateService(out _);

        Assert.False(service.IsCapturing);

        service.BeginCapture("jump", InputDeviceKind.Gamepad);
        Assert.True(service.IsCapturing);
        Assert.Equal("jump", service.CapturingAction);
        Assert.Equal(InputDeviceKind.Gamepad, service.CapturingDeviceKind);

        var cancelled = false;
        service.CaptureCancelled += () => cancelled = true;
        service.CancelCapture();

        Assert.False(service.IsCapturing);
        Assert.True(cancelled);
    }

    [Fact]
    public void 服务_拒绝对不可改键动作或不支持的设备开始捕获()
    {
        var service = CreateService(out _);

        Assert.Throws<ArgumentException>(() => service.BeginCapture("不存在", InputDeviceKind.KeyboardMouse));
        Assert.Throws<ArgumentException>(() => service.BeginCapture("jump", InputDeviceKind.Touch));
    }

    [Fact]
    public void 快照_序列化与反序列化往返保持一致()
    {
        var service = CreateService(out _);

        var json = InputBindingSnapshotFile.Serialize(service.ExportSnapshot());
        var restored = InputBindingSnapshotFile.Deserialize(json);

        Assert.NotNull(restored);
        Assert.Equal(2, restored!.Actions.Count);

        var jump = restored.Actions.First(static action => action.ActionName == "jump");
        Assert.Equal("key:32", jump.Bindings[0].Code);
        Assert.Equal("Space", jump.Bindings[0].DisplayName);
        Assert.Equal(InputDeviceKind.KeyboardMouse, jump.Bindings[0].DeviceKind);
        Assert.Equal(InputBindingKind.Key, jump.Bindings[0].BindingKind);
    }

    [Fact]
    public void 快照_轴绑定的方向在序列化后保留()
    {
        var snapshot = new InputBindingSnapshot(
        [
            new InputActionBinding("move_left",
                [InputBindingDescriptorFactory.ForGamepadAxis(0, -1f, "左摇杆 ←")]),
        ]);

        var restored = InputBindingSnapshotFile.Deserialize(InputBindingSnapshotFile.Serialize(snapshot));

        var binding = restored!.Actions[0].Bindings[0];
        Assert.Equal("joy-axis:0:-1", binding.Code);
        Assert.Equal(-1f, binding.AxisDirection);
        Assert.Equal("左摇杆 ←", binding.DisplayName);
    }

    [Fact]
    public void 快照_非法输入返回空而不抛异常()
    {
        Assert.Null(InputBindingSnapshotFile.Deserialize(""));
        Assert.Null(InputBindingSnapshotFile.Deserialize("   "));
        Assert.Null(InputBindingSnapshotFile.Deserialize("{ 这不是合法 JSON"));
    }

    [Fact]
    public void 快照_导入后可被服务读取()
    {
        var service = CreateService(out _);
        var imported = new InputBindingSnapshot(
        [
            new InputActionBinding("jump",
                [InputBindingDescriptorFactory.ForGamepadButton(0, "A"), InputBindingDescriptorFactory.ForKey(32, "Space")]),
        ]);

        service.ImportSnapshot(imported);

        var gamepad = service.GetPrimaryBinding("jump", InputDeviceKind.Gamepad);
        Assert.Equal("joy-button:0", gamepad!.Code);

        var keyboard = service.GetPrimaryBinding("jump", InputDeviceKind.KeyboardMouse);
        Assert.Equal("key:32", keyboard!.Code);
    }
}
