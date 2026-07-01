using GFrameworkTemplate.scripts.core.story;

namespace GFrameworkTemplate.scripts.system.visualnovel;

/// <summary>
///     即发即忘型 Worker 基类——发送事件后不等待
/// </summary>
public abstract class FireAndForgetWorker<TCommand, TEvent> : IStoryCommandWorker
    where TCommand : StoryCommand
    where TEvent : class
{
    public Task ExecuteAsync(StoryCommand cmd, EngineContext ctx)
    {
        ctx.SendEvent(CreateEvent((TCommand)cmd));
        return Task.CompletedTask;
    }

    protected abstract TEvent CreateEvent(TCommand cmd);
}
