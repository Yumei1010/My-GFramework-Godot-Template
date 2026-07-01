using GFrameworkTemplate.scripts.cqrs.visualnovel.command;
using GFrameworkTemplate.scripts.cqrs.visualnovel.@event;

namespace GFrameworkTemplate.scripts.system.visualnovel;

public sealed class TachieWorker : FireAndForgetWorker<TachieCommand, VisualNovelTachieTriggeredEvent>
{
    protected override VisualNovelTachieTriggeredEvent CreateEvent(TachieCommand cmd) =>
        new() { Tachies = cmd.Tachies };
}
