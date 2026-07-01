using GFrameworkTemplate.scripts.core.story;
using GFrameworkTemplate.scripts.system.visualnovel;
using GFrameworkTemplate.scripts.cqrs.visualnovel.command;
using GFrameworkTemplate.scripts.cqrs.visualnovel.@event;

namespace GFrameworkTemplate.scripts.entities.story_command_worker;

public sealed class EventWorker : IStoryCommandWorker
{
    public async Task ExecuteAsync(StoryCommand cmd, EngineContext ctx)
    {
        var evt = (EventCommand)cmd;
        ctx.SendEvent(new VisualNovelCustomEventTriggeredEvent { EventName = evt.EventName });
        await ctx.AdvanceAsync(0.1f);
    }
}
