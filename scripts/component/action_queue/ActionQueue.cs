using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GFrameworkTemplate.scripts.component.action_queue;

/// <summary>
///     动作队列：按序串行执行异步步骤（前一个完成才执行下一个）。
///     契合"按序播放动画/连锁效果"场景：把动画步骤排入队列，自动逐个执行。
/// </summary>
/// <remarks>
///     纯逻辑组件，零 Godot / GFramework 依赖，可在单元测试中使用；队列状态受锁保护，可跨线程入队。
///     <para>
///     失败策略：某一步抛出异常时，**中断执行并丢弃剩余步骤**，同时抛出
///     <see cref="OnStepFailed" /> 事件，异常会通过 <see cref="WaitUntilIdleAsync" /> 向等待方传播
///     （未被观察也不影响后续入队）。
///     </para>
///     <para>
///     用法示例：
///     <code>
///     var queue = new ActionQueue();
///     queue.Enqueue(async () =&gt; { await MoveToTargetAsync(); });   // 第 1 步：移动对象
///     queue.Enqueue(async () =&gt; { await PlayAnimationAsync(); });  // 第 2 步：播放动画
///     queue.Enqueue(() =&gt; CalculateScore());                       // 第 3 步：计分
///     await queue.WaitUntilIdleAsync();                            // 等待整条链执行完毕
///     // 自动串行执行：移动完成 → 动画完成 → 计分
///     </code>
///     </para>
/// </remarks>
public sealed class ActionQueue
{
    private readonly object _gate = new();
    private readonly Queue<Func<Task>> _pending = new();
    private TaskCompletionSource? _idle;
    private bool _isRunning;

    /// <summary>
    ///     某一步骤执行失败时触发（参数为该步骤抛出的异常）。
    /// </summary>
    /// <remarks>
    ///     无论是否订阅该事件，异常都会通过 <see cref="WaitUntilIdleAsync" /> 传播给等待方。
    /// </remarks>
    public event Action<Exception>? OnStepFailed;

    /// <summary>
    ///     队列是否正在执行（有步骤运行中或待执行）。
    /// </summary>
    public bool IsRunning
    {
        get
        {
            lock (_gate)
            {
                return _isRunning;
            }
        }
    }

    /// <summary>
    ///     当前待执行步骤数（不含正在执行的）。
    /// </summary>
    public int PendingCount
    {
        get
        {
            lock (_gate)
            {
                return _pending.Count;
            }
        }
    }

    /// <summary>
    ///     是否为空（无待执行且未在运行）。
    /// </summary>
    public bool IsEmpty
    {
        get
        {
            lock (_gate)
            {
                return !_isRunning && _pending.Count == 0;
            }
        }
    }

    /// <summary>
    ///     排入一个异步步骤。若队列空闲则立即开始执行，否则等待前序完成。
    /// </summary>
    /// <param name="step">异步步骤，返回的 Task 完成表示该步骤结束。</param>
    /// <exception cref="ArgumentNullException">当 <paramref name="step" /> 为 <see langword="null" /> 时抛出。</exception>
    public void Enqueue(Func<Task> step)
    {
        ArgumentNullException.ThrowIfNull(step);

        var shouldStart = false;
        lock (_gate)
        {
            _pending.Enqueue(step);
            if (!_isRunning)
            {
                _isRunning = true;
                _idle ??= new TaskCompletionSource();
                shouldStart = true;
            }
        }

        if (shouldStart)
        {
            _ = RunAsync();
        }
    }

    /// <summary>
    ///     清空所有待执行步骤（正在执行的步骤不受影响）。
    /// </summary>
    public void Clear()
    {
        lock (_gate)
        {
            _pending.Clear();
        }
    }

    /// <summary>
    ///     等待队列执行完毕（无待执行且未在运行）。
    /// </summary>
    /// <returns>队列空闲时完成；若期间某步骤失败，则该 Task 以该异常失败。</returns>
    public Task WaitUntilIdleAsync()
    {
        lock (_gate)
        {
            if (!_isRunning && _pending.Count == 0)
            {
                return Task.CompletedTask;
            }

            return (_idle ??= new TaskCompletionSource()).Task;
        }
    }

    /// <summary>
    ///     串行取出并执行步骤，直到队列清空、被清空或某一步失败。
    /// </summary>
    private async Task RunAsync()
    {
        Exception? failure = null;

        while (true)
        {
            Func<Task>? step;
            lock (_gate)
            {
                if (_pending.Count == 0)
                {
                    break;
                }

                step = _pending.Dequeue();
            }

            try
            {
                await step().ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                failure = exception;
                lock (_gate)
                {
                    // 失败即丢弃剩余步骤，避免在前置条件未满足时继续执行后续动作
                    _pending.Clear();
                }

                break;
            }
        }

        TaskCompletionSource? idle;
        lock (_gate)
        {
            _isRunning = false;
            idle = _idle;
            _idle = null;
        }

        if (failure is null)
        {
            idle?.TrySetResult();
            return;
        }

        OnStepFailed?.Invoke(failure);
        idle?.TrySetException(failure);
    }
}
