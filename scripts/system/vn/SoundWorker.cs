using GFrameworkTemplate.scripts.core.story;
using GFrameworkTemplate.scripts.cqrs.vn.@event;

namespace GFrameworkTemplate.scripts.system.vn;

/// <summary>
///     音频命令执行器——即发即忘
/// </summary>
public sealed class SoundWorker : IStoryCommandWorker
{
    public Task ExecuteAsync(StoryCommand cmd, EngineContext ctx)
    {
        var sound = (SoundCommand)cmd;
        ctx.SendEvent(new VnSoundTriggeredEvent
        {
            SoundType = sound.SoundType,
            FilePath = sound.FilePath ?? string.Empty
        });
        return Task.CompletedTask;
    }
}
