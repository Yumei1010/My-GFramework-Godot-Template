namespace GFrameworkTemplate.scripts.system.vn;

/// <summary>
///     引擎等待工具——统一"等待推进或自动播放"逻辑，供 Worker 复用
/// </summary>
public static class EngineAwait
{
    /// <summary>
    ///     等待玩家点击推进，或自动播放计时器触发
    /// </summary>
    /// <param name="minDuration">最小等待时间（打字机效果等）</param>
    /// <param name="ctx">引擎上下文</param>
    public static async Task Advance(float minDuration, EngineContext ctx)
    {
        if (ctx.AutoPlayDelay.HasValue)
        {
            var waitTime = Math.Max(minDuration, ctx.AutoPlayDelay.Value);
            await Task.Delay(TimeSpan.FromSeconds(waitTime));
        }
        else
        {
            ctx.WaitSource = new TaskCompletionSource<bool>();
            await ctx.WaitSource.Task;
            ctx.WaitSource = null;
        }
    }
}
