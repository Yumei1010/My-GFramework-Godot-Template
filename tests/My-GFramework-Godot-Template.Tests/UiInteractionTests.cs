using GFramework.Game.Abstractions.Enums;
using GFramework.Game.Abstractions.UI;
using GFramework.Game.UI;

namespace GFrameworkTemplate.Tests;

/// <summary>
///     UI 交互档案语义验证：docs/guides/ui.md 中的弹窗输入/阻断语义。
///     UiInteractionProfiles 是纯静态 helper，可脱离 Godot 测。
/// </summary>
public class UiInteractionTests
{
    [Fact]
    public void 预置档案_Default不捕获不阻断()
    {
        var profile = UiInteractionProfiles.Default;

        Assert.Equal(UiInputActionMask.None, profile.CapturedActions);
        Assert.False(profile.BlocksWorldPointerInput);
        Assert.False(profile.BlocksWorldActionInput);
    }

    [Fact]
    public void 预置档案_BlockingCancel捕获Cancel并阻断()
    {
        var profile = UiInteractionProfiles.BlockingCancel;

        Assert.True(UiInteractionProfiles.Captures(profile, UiInputAction.Cancel));
        Assert.False(UiInteractionProfiles.Captures(profile, UiInputAction.Confirm));
        Assert.True(profile.BlocksWorldPointerInput);
        Assert.True(profile.BlocksWorldActionInput);
    }

    [Theory]
    [InlineData(UiLayer.Modal)]
    [InlineData(UiLayer.Topmost)]
    public void CreateDefault_模态与最高层用阻塞档案(UiLayer layer)
    {
        var profile = UiInteractionProfiles.CreateDefault(layer);
        Assert.True(UiInteractionProfiles.Captures(profile, UiInputAction.Cancel));
        Assert.True(profile.BlocksWorldPointerInput);
    }

    [Theory]
    [InlineData(UiLayer.Page)]
    [InlineData(UiLayer.Overlay)]
    [InlineData(UiLayer.Toast)]
    public void CreateDefault_其余层用默认档案(UiLayer layer)
    {
        var profile = UiInteractionProfiles.CreateDefault(layer);
        Assert.False(UiInteractionProfiles.Captures(profile, UiInputAction.Cancel));
        Assert.False(profile.BlocksWorldPointerInput);
    }

    [Fact]
    public void 自定义档案_可按动作捕获()
    {
        var profile = new UiInteractionProfile
        {
            CapturedActions = UiInputActionMask.Confirm,
            BlocksWorldActionInput = true
        };

        Assert.True(UiInteractionProfiles.Captures(profile, UiInputAction.Confirm));
        Assert.False(UiInteractionProfiles.Captures(profile, UiInputAction.Cancel));
        Assert.False(profile.BlocksWorldPointerInput);
        Assert.True(profile.BlocksWorldActionInput);
    }
}