using System;
using GFrameworkTemplate.scripts.component.behavior_tree;

namespace GFrameworkTemplate.Tests;

/// <summary>
///     行为树上下文与黑板单元测试。
/// </summary>
/// <remarks>
///     行为树节点本身是 Godot 节点（需 Godot 运行时），其执行逻辑通过编辑器场景验证；
///     此处覆盖可脱离 Godot 的纯逻辑部分：<see cref="Blackboard" /> 与 <see cref="BehaviorContext" />。
/// </remarks>
public class BehaviorTreeContextTests
{
    [Fact]
    public void Blackboard_SetAndGet_RoundTrips()
    {
        var board = new Blackboard();

        board.Set("target", "goblin");
        board.Set("ammo", 3);

        Assert.Equal("goblin", board.Get<string>("target"));
        Assert.Equal(3, board.Get<int>("ammo"));
        Assert.Equal(2, board.Count);
    }

    [Fact]
    public void Blackboard_MissingKey_ReturnsDefault()
    {
        var board = new Blackboard();

        Assert.Null(board.Get<string>("nope"));
        Assert.Equal(0, board.Get<int>("nope"));
        Assert.False(board.ContainsKey("nope"));
    }

    [Fact]
    public void Blackboard_TryGet_ReportsTypeMismatch()
    {
        var board = new Blackboard();
        board.Set("value", 42);

        Assert.True(board.TryGet<int>("value", out var asInt));
        Assert.Equal(42, asInt);
        Assert.False(board.TryGet<string>("value", out _));   // 类型不匹配
    }

    [Fact]
    public void Blackboard_SetOverwrites_AndRemoveClears()
    {
        var board = new Blackboard();
        board.Set("hp", 100);
        board.Set("hp", 50);
        Assert.Equal(50, board.Get<int>("hp"));

        Assert.True(board.Remove("hp"));
        Assert.False(board.Remove("hp"));
        Assert.False(board.ContainsKey("hp"));
    }

    [Fact]
    public void Blackboard_Clear_EmptiesBoard()
    {
        var board = new Blackboard();
        board.Set("a", 1);
        board.Set("b", 2);

        board.Clear();

        Assert.Equal(0, board.Count);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Blackboard_BlankKey_Throws(string key)
    {
        var board = new Blackboard();

        Assert.ThrowsAny<ArgumentException>(() => board.Set(key, 1));
        Assert.ThrowsAny<ArgumentException>(() => board.Get<int>(key));
    }

    [Fact]
    public void BehaviorContext_DeltaIsWritable_AndHasBlackboard()
    {
        var context = new BehaviorContext();

        context.Delta = 0.016;
        context.Blackboard.Set("shared", "data");

        Assert.Equal(0.016, context.Delta);
        Assert.Equal("data", context.Blackboard.Get<string>("shared"));
    }
}
