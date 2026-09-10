using System;
using System.Collections.Generic;
using GFramework.Core.Abstractions.Pause;
using GFramework.Core.Pause;

namespace GFrameworkTemplate.Tests;

/// <summary>
///     暂停栈验证：文档 docs/guides/pause.md 的核心语义。
///     PauseStackManager 是纯逻辑类，可脱离 Godot 直接测。
/// </summary>
public class PauseStackTests
{
    private sealed class RecordingHandler : IPauseHandler
    {
        public int Priority { get; set; }
        public readonly List<(PauseGroup Group, bool IsPaused)> Calls = new();

        public void OnPauseStateChanged(PauseGroup group, bool isPaused)
        {
            Calls.Add((group, isPaused));
        }
    }

    private static PauseStackManager CreateManager() => new();

    [Fact]
    public void Push_Pop_基础暂停恢复()
    {
        var manager = CreateManager();
        Assert.False(manager.IsPaused(PauseGroup.Gameplay));

        var token = manager.Push("打开设置", PauseGroup.Gameplay);
        Assert.True(manager.IsPaused(PauseGroup.Gameplay));
        Assert.Equal(1, manager.GetPauseDepth(PauseGroup.Gameplay));

        Assert.True(manager.Pop(token));
        Assert.False(manager.IsPaused(PauseGroup.Gameplay));
    }

    [Fact]
    public void 分组独立_Global与Gameplay互不干扰()
    {
        var manager = CreateManager();
        manager.Push("暂停玩法", PauseGroup.Gameplay);

        Assert.True(manager.IsPaused(PauseGroup.Gameplay));
        Assert.False(manager.IsPaused(PauseGroup.Global));   // Global 未暂停
        Assert.False(manager.IsPaused(PauseGroup.Animation));
    }

    [Fact]
    public void 嵌套暂停_引用计数()
    {
        var manager = CreateManager();
        var a = manager.Push("弹窗A");
        var b = manager.Push("弹窗B");

        Assert.Equal(2, manager.GetPauseDepth(PauseGroup.Global));
        Assert.True(manager.IsPaused(PauseGroup.Global));

        manager.Pop(a);   // 非栈顶也允许
        Assert.True(manager.IsPaused(PauseGroup.Global));   // b 还在

        manager.Pop(b);
        Assert.False(manager.IsPaused(PauseGroup.Global));
    }

    [Fact]
    public void 重复Pop_幂等安全()
    {
        var manager = CreateManager();
        var token = manager.Push("x");
        Assert.True(manager.Pop(token));
        Assert.False(manager.Pop(token));   // 第二次返回 false
    }

    [Fact]
    public void 处理器_边界通知与优先级()
    {
        var manager = CreateManager();
        var handler = new RecordingHandler();
        manager.RegisterHandler(handler);

        var token = manager.Push("p", PauseGroup.Gameplay);
        manager.Pop(token);

        Assert.Equal(2, handler.Calls.Count);
        Assert.Equal((PauseGroup.Gameplay, true), handler.Calls[0]);   // 暂停通知
        Assert.Equal((PauseGroup.Gameplay, false), handler.Calls[1]);  // 恢复通知
    }

    [Fact]
    public void 嵌套暂停_只在边界通知一次()
    {
        var manager = CreateManager();
        var handler = new RecordingHandler();
        manager.RegisterHandler(handler);

        var a = manager.Push("a");
        var b = manager.Push("b");   // 已暂停 → 不通知
        Assert.Equal(1, handler.Calls.Count);

        manager.Pop(b);   // 仍暂停 → 不通知
        Assert.Equal(1, handler.Calls.Count);

        manager.Pop(a);   // 最后一个 → 恢复通知
        Assert.Equal(2, handler.Calls.Count);
        Assert.Equal((PauseGroup.Global, false), handler.Calls[1]);
    }

    [Fact]
    public void PauseScope_自动PushPop()
    {
        var manager = CreateManager();
        using (manager.PauseScope("结算中", PauseGroup.Gameplay))
        {
            Assert.True(manager.IsPaused(PauseGroup.Gameplay));
        }
        Assert.False(manager.IsPaused(PauseGroup.Gameplay));
    }

    [Fact]
    public void GetPauseReasons_诊断可查()
    {
        var manager = CreateManager();
        manager.Push("弹窗A");
        manager.Push("弹窗B");

        var reasons = manager.GetPauseReasons(PauseGroup.Global);
        Assert.Contains("弹窗A", reasons);
        Assert.Contains("弹窗B", reasons);
    }

    [Fact]
    public void ClearGroup_强制清空并通知恢复()
    {
        var manager = CreateManager();
        var handler = new RecordingHandler();
        manager.RegisterHandler(handler);

        manager.Push("a");
        manager.Push("b");
        manager.ClearGroup(PauseGroup.Global);

        Assert.False(manager.IsPaused(PauseGroup.Global));
        Assert.Equal(0, manager.GetPauseDepth(PauseGroup.Global));
        Assert.Equal((PauseGroup.Global, false), handler.Calls[^1]);   // 恢复通知
    }
}