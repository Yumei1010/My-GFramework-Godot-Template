using GFrameworkTemplate.scripts.core.story;
using GFrameworkTemplate.scripts.cqrs.vn.@event;

namespace GFrameworkTemplate.scripts.system.vn;

/// <summary>
///     立绘命令执行器——即发即忘，不等待
/// </summary>
public sealed class TachieWorker : IStoryCommandWorker
{
    public Task ExecuteAsync(StoryCommand cmd, EngineContext ctx)
    {
        var tachie = (TachieCommand)cmd;
        ctx.SendEvent(new VnTachieTriggeredEvent { Tachies = tachie.Tachies });
        return Task.CompletedTask;
    }
}
