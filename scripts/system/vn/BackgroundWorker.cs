using GFrameworkTemplate.scripts.core.story;
using GFrameworkTemplate.scripts.cqrs.vn.@event;

namespace GFrameworkTemplate.scripts.system.vn;

/// <summary>
///     背景命令执行器——发送背景切换事件
/// </summary>
public sealed class BackgroundWorker : IStoryCommandWorker
{
    public async Task ExecuteAsync(StoryCommand cmd, EngineContext ctx)
    {
        var bg = (BackgroundCommand)cmd;

        if (bg.Delay > 0)
            await Task.Delay(TimeSpan.FromSeconds(bg.Delay));

        ctx.SendEvent(new VnBackgroundTriggeredEvent
        {
            FilePath = bg.FilePath ?? string.Empty,
            WaitTweenEnd = bg.WaitTweenEnd,
            Delay = bg.Delay
        });

        if (bg.WaitTweenEnd)
            await Task.Delay(TimeSpan.FromSeconds(0.5f));
    }
}
