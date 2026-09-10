using System;
using System.Collections;
using System.Collections.Generic;
using GFramework.Core.Abstractions.Coroutine;
using GFramework.Core.Coroutine;
using GFramework.Core.Coroutine.Extensions;
using GFramework.Core.Coroutine.Instructions;

namespace GFrameworkTemplate.Tests;

/// <summary>
///     协程系统验证：文档 docs/guides/coroutine.md 中的核心 API 用法。
///     使用纯 Core 的 CoroutineScheduler（不依赖 Godot 运行时），手动驱动 Update。
/// </summary>
public class CoroutineSystemTests
{
    private sealed class TestTimeSource : ITimeSource
    {
        public double CurrentTime { get; private set; }
        public double DeltaTime { get; private set; }

        public void Update()
        {
            DeltaTime = 0.016;
            CurrentTime += DeltaTime;
        }
    }

    private static CoroutineScheduler CreateScheduler(out TestTimeSource timeSource)
    {
        timeSource = new TestTimeSource();
        return new CoroutineScheduler(timeSource, initialCapacity: 8);
    }

    [Fact]
    public void 基础驱动_等待时间后执行下一步()
    {
        var scheduler = CreateScheduler(out _);
        var executed = false;

        var coroutine = Flow();
        scheduler.Run(coroutine);
        Assert.False(executed, "不能立刻执行，要先等 0.5s");

        for (var i = 0; i < 60; i++) scheduler.Update();   // 60 帧 ≈ 1s
        Assert.True(executed, "0.5s 后应执行");

        return;

        IEnumerator<IYieldInstruction> Flow()
        {
            yield return new WaitForSecondsScaled(0.5);
            executed = true;
        }
    }

    [Fact]
    public void WaitUntil_等条件成立()
    {
        var scheduler = CreateScheduler(out _);
        var flag = false;
        var done = false;

        var coroutine = Flow();
        scheduler.Run(coroutine);

        for (var i = 0; i < 10; i++) scheduler.Update();
        Assert.False(done, "条件未成立前不应完成");

        flag = true;
        for (var i = 0; i < 10; i++) scheduler.Update();
        Assert.True(done, "条件成立后应完成");

        return;

        IEnumerator<IYieldInstruction> Flow()
        {
            yield return new WaitUntil(() => flag);
            done = true;
        }
    }

    [Fact]
    public void WaitForFrames_等N帧()
    {
        var scheduler = CreateScheduler(out _);
        var frame = 0;
        var done = false;

        var coroutine = Flow();
        scheduler.Run(coroutine);

        for (var i = 0; i < 3; i++) { scheduler.Update(); frame++; }
        Assert.False(done, "3 帧后应差一帧（等 5 帧）");

        for (var i = 0; i < 3; i++) { scheduler.Update(); frame++; }
        Assert.True(done, "6 帧后应完成");

        return;

        IEnumerator<IYieldInstruction> Flow()
        {
            yield return new WaitForFrames(5);
            done = true;
        }
    }

    [Fact]
    public void CoroutineHelper_工厂可用()
    {
        // WaitForSeconds 直接返回指令实例
        var delay = CoroutineHelper.WaitForSeconds(1.0);
        Assert.False(delay.IsDone);
        delay.Update(1.0);
        Assert.True(delay.IsDone);

        // Delay 计数走完
        var calls = 0;
        var delayed = CoroutineHelper.DelayedCall(0.5, () => calls++);
        while (delayed.MoveNext()) delayed.Current.Update(0.1);
        Assert.Equal(1, calls);
    }

    [Fact]
    public void 组合扩展_Then顺序执行()
    {
        var scheduler = CreateScheduler(out _);
        var order = new List<string>();

        // Then 接受两个枚举器；DelayedCall 返回枚举器可直接链 Then
        var coroutine = CoroutineHelper.DelayedCall(0.01, () => { }).Then(StepTwo());
        scheduler.Run(coroutine);

        for (var i = 0; i < 10; i++) scheduler.Update();
        Assert.Equal(new[] { "step1", "step2" }, order);

        return;

        IEnumerator<IYieldInstruction> StepTwo()
        {
            order.Add("step1");
            yield return new WaitForNextFrame();
            order.Add("step2");
        }
    }
}