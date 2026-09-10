using System;
using System.Collections.Generic;
using GFramework.Core.Abstractions.Property;
using GFramework.Core.Abstractions.StateManagement;
using GFramework.Core.Extensions;
using GFramework.Core.StateManagement;

namespace GFrameworkTemplate.Tests;

// 临时验证：Store 教学文档 docs/guides/store.md 中的 API 用法是否真实可用
public class StoreDocVerificationTests
{
    public sealed record RunState(int Score, int Hp, int Level, bool IsCleared);
    public sealed record RoundWonAction(int Bonus);
    public sealed record RoundLostAction();

    public sealed class LoggingMiddleware : IStoreMiddleware<RunState>
    {
        public readonly List<string> Logs = new();

        public void Invoke(StoreDispatchContext<RunState> ctx, Action next)
        {
            Logs.Add($"提交: {ctx.ActionType.Name}");
            next();
            Logs.Add($"变化: {ctx.HasStateChanged}，关卡 {ctx.PreviousState.Level}→{ctx.NextState.Level}");
        }
    }

    private static Store<RunState> NewStore(int history = 0)
    {
        return new Store<RunState>(new RunState(0, 3, 1, false), historyCapacity: history)
            .RegisterReducer<RoundWonAction>((s, a) => s with
            {
                Score = s.Score + a.Bonus,
                Level = s.Level + 1,
                IsCleared = s.Level + 1 >= 5
            })
            .RegisterReducer<RoundLostAction>((s, _) => s with { Hp = s.Hp - 1 });
    }

    [Fact]
    public void 基本_Dispatch与Subscribe()
    {
        var store = NewStore();
        var seen = new List<int>();
        store.SubscribeWithInitValue(s => seen.Add(s.Score));

        store.Dispatch(new RoundWonAction(100));
        store.Dispatch(new RoundWonAction(50));

        Assert.Equal(new[] { 0, 100, 150 }, seen);
        Assert.Equal(150, store.State.Score);
        Assert.Equal(3, store.State.Level);
    }

    [Fact]
    public void Selector_只通知变化片段()
    {
        var store = NewStore();
        var scoreHits = 0;
        var hpValue = 0;

        store.Select(s => s.Score).RegisterWithInitValue(_ => scoreHits++);
        store.Select(s => s.Hp).RegisterWithInitValue(v => hpValue = v);

        // 过关：Score 变、Hp 不变 → scoreHits 增，hpValue 不变
        store.Dispatch(new RoundWonAction(100));
        Assert.Equal(2, scoreHits);   // 初始 1 次 + 变化 1 次
        Assert.Equal(3, hpValue);

        // 失败：Hp 变、Score 不变
        store.Dispatch(new RoundLostAction());
        Assert.Equal(2, hpValue);
        Assert.Equal(2, scoreHits);   // Score 没变，不通知
    }

    [Fact]
    public void ToBindableProperty_桥接可用()
    {
        var store = NewStore();
        IReadonlyBindableProperty<int> hp = store.ToBindableProperty(s => s.Hp);
        Assert.Equal(3, hp.Value);
    }

    [Fact]
    public void GetOrCreateBindableProperty_同Key复用实例()
    {
        var store = NewStore();
        var a = store.GetOrCreateBindableProperty("hp", s => s.Hp);
        var b = store.GetOrCreateBindableProperty("hp", s => s.Hp);
        Assert.Same(a, b);
    }

    [Fact]
    public void Middleware_洋葱包裹并可见前后状态()
    {
        var store = NewStore();
        var mw = new LoggingMiddleware();
        store.UseMiddleware(mw);

        store.Dispatch(new RoundWonAction(100));

        Assert.Equal(2, mw.Logs.Count);
        Assert.Contains("提交: RoundWonAction", mw.Logs[0]);
        Assert.Contains("1→2", mw.Logs[1]);
    }

    [Fact]
    public void History_撤销重做时间旅行()
    {
        var store = NewStore(history: 50);
        var initial = store.State;

        store.Dispatch(new RoundWonAction(100));
        store.Dispatch(new RoundWonAction(100));
        Assert.Equal(200, store.State.Score);
        Assert.True(store.CanUndo);

        store.Undo();
        Assert.Equal(100, store.State.Score);
        Assert.True(store.CanRedo);

        store.Redo();
        Assert.Equal(200, store.State.Score);

        store.TimeTravelTo(0);
        Assert.Equal(initial.Score, store.State.Score);
    }

    [Fact]
    public void RunInBatch_通知折叠为一次()
    {
        var store = NewStore();
        var notifications = 0;
        store.Subscribe(_ => notifications++);

        store.RunInBatch(() =>
        {
            store.Dispatch(new RoundWonAction(10));
            store.Dispatch(new RoundWonAction(10));
            store.Dispatch(new RoundLostAction());
        });

        Assert.Equal(1, notifications);          // 只通知一次
        Assert.Equal(20, store.State.Score);     // 状态都提交了
        Assert.Equal(2, store.State.Hp);
    }

    [Fact]
    public void 诊断_可读取订阅数与最后动作()
    {
        var store = NewStore();
        store.Subscribe(_ => { });
        store.Dispatch(new RoundWonAction(10));

        Assert.Equal(1, store.SubscriberCount);
        Assert.Equal(typeof(RoundWonAction), store.LastActionType);
        Assert.NotNull(store.LastDispatchRecord);
    }

    [Fact]
    public void 运行时句柄_可注销中间件()
    {
        var store = NewStore();
        var mw = new LoggingMiddleware();
        var handle = store.RegisterMiddleware(mw);

        store.Dispatch(new RoundWonAction(10));
        Assert.Equal(2, mw.Logs.Count);

        handle.UnRegister();
        store.Dispatch(new RoundWonAction(10));
        Assert.Equal(2, mw.Logs.Count);   // 注销后不再记录
    }

    [Fact]
    public void StoreBuilder_链式构建()
    {
        var store = Store<RunState>.CreateBuilder()
            .WithHistoryCapacity(10)
            .AddReducer<RoundWonAction>((s, a) => s with { Score = s.Score + a.Bonus })
            .UseMiddleware(new LoggingMiddleware())
            .Build(new RunState(0, 3, 1, false));

        store.Dispatch(new RoundWonAction(5));
        Assert.Equal(5, store.State.Score);

        // 诊断成员在 IStoreDiagnostics<TState> 上（Builder.Build 返回 IStore，需要转型）
        var diagnostics = (IStoreDiagnostics<RunState>)store;
        Assert.Equal(10, diagnostics.HistoryCapacity);
    }

    [Fact]
    public void 精确类型匹配_默认不响应继承层次()
    {
        var store = NewStore();
        store.Dispatch(new RoundWonAction(10));
        // 精确类型外的不匹配 action 不改变状态
        Assert.Equal(10, store.State.Score);
        Assert.Equal(StoreActionMatchingMode.ExactTypeOnly, store.ActionMatchingMode);
    }
}
