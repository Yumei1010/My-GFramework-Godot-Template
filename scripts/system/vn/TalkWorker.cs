using GFrameworkTemplate.scripts.core.story;
using GFrameworkTemplate.scripts.cqrs.vn.@event;

namespace GFrameworkTemplate.scripts.system.vn;

/// <summary>
///     对话命令执行器——发送对话事件并等待玩家推进
/// </summary>
public sealed class TalkWorker : IStoryCommandWorker
{
    public async Task ExecuteAsync(StoryCommand cmd, EngineContext ctx)
    {
        var talk = (TalkCommand)cmd;

        ctx.SendEvent(new VnTalkTriggeredEvent
        {
            Talker = talk.Talker,
            Content = talk.TalkContent,
            IsCenter = talk.IsCenter,
            AvatarPath = talk.AvatarPath
        });

        ctx.SendEvent(new VnTextRevealProgressEvent
        {
            RevealedChars = 0,
            TotalChars = talk.TalkContent.Length
        });

        await EngineAwait.Advance(talk.TalkContent.Length * ctx.WordSpeed, ctx);
    }
}
