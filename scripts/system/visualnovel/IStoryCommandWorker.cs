using GFrameworkTemplate.scripts.core.story;
using GFrameworkTemplate.scripts.cqrs.visualnovel.command;

namespace GFrameworkTemplate.scripts.system.visualnovel;

public interface IStoryCommandWorker
{
    Task ExecuteAsync(StoryCommand cmd, EngineContext ctx);
}
