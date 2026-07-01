using GFrameworkTemplate.scripts.core.story;
using GFrameworkTemplate.scripts.cqrs.vn.@event;

namespace GFrameworkTemplate.scripts.system.vn;

/// <summary>
///     分支命令执行器——发送选项并等待玩家选择
/// </summary>
public sealed class BranchWorker : IStoryCommandWorker
{
    public async Task ExecuteAsync(StoryCommand cmd, EngineContext ctx)
    {
        var branch = (BranchCommand)cmd;
        ctx.SendEvent(new VnBranchTriggeredEvent { Options = branch.Options });

        var tcs = new TaskCompletionSource<string?>();
        var sub = ctx.RegisterEvent<VnBranchChosenEvent>(e => tcs.TrySetResult(e.OptionId));

        var chosenId = await tcs.Task;
        sub.UnRegister();

        if (chosenId != null)
            ctx.TalkBranch.Add(chosenId);
    }
}
