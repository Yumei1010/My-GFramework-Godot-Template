using GFrameworkTemplate.scripts.core.story;
using GFrameworkTemplate.scripts.cqrs.visualnovel.command;

namespace GFrameworkTemplate.scripts.entities.story_command_worker;

public interface IStoryCommandWorker
{
    Task ExecuteAsync(StoryCommand cmd, EngineContext ctx);
}
