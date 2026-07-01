using GFrameworkTemplate.scripts.core.story;
using GFrameworkTemplate.scripts.cqrs.vn.@event;

namespace GFrameworkTemplate.scripts.system.vn;

/// <summary>
///     Goto 命令执行器——标记引擎停止并发送跳转事件
///     实际的重新加载由 StoryEngineSystem 的 PlayLoop 检测处理
/// </summary>
public sealed class GotoWorker : IStoryCommandWorker
{
    public Task ExecuteAsync(StoryCommand cmd, EngineContext ctx)
    {
        var gt = (GotoCommand)cmd;
        var target = gt.FilePath;

        if (string.IsNullOrEmpty(target))
            return Task.CompletedTask;

        ctx.SendEvent(new VnGotoTriggeredEvent { TargetFilePath = target });
        ctx.IsPlaying = false; // 终止当前 PlayLoop
        ctx.PendingGoto = target;

        return Task.CompletedTask;
    }
}
