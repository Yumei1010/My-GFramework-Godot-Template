using GFrameworkTemplate.scripts.core.story;

namespace GFrameworkTemplate.scripts.system.vn;

/// <summary>
///     命令执行器接口——每个命令类型对应一个 Worker
/// </summary>
public interface IStoryCommandWorker
{
    /// <summary>异步执行命令</summary>
    Task ExecuteAsync(StoryCommand cmd, EngineContext ctx);
}
