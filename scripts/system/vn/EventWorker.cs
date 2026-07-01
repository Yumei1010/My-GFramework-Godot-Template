using GFrameworkTemplate.scripts.core.story;
using GFrameworkTemplate.scripts.cqrs.vn.@event;

namespace GFrameworkTemplate.scripts.system.vn;

/// <summary>
///     自定义事件命令执行器——发送事件并短暂等待
/// </summary>
public sealed class EventWorker : IStoryCommandWorker
{
    public async Task ExecuteAsync(StoryCommand cmd, EngineContext ctx)
    {
        var evt = (EventCommand)cmd;
        ctx.SendEvent(new VnCustomEventTriggeredEvent { EventName = evt.EventName });
        await EngineAwait.Advance(0.1f, ctx);
    }
}
