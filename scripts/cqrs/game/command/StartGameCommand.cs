using System.Threading.Tasks;
using GFramework.Core.Command;
using GFramework.Core.Extensions;
using GFrameworkTemplate.scripts.cqrs.game.@event;

namespace GFrameworkTemplate.scripts.cqrs.game.command;

/// <summary>
///     开始游戏命令：发送 <see cref="GameStartedEvent" />，演示命令到事件的链路。
/// </summary>
/// <remarks>
///     Godot 主线程禁止同步 CQRS 调用（框架 GuardSyncCqrs），因此命令统一使用异步形式
///     （与 audio/setting 域命令保持一致）。
/// </remarks>
public sealed class StartGameCommand : AbstractAsyncCommand
{
    /// <summary>
    ///     执行开始游戏逻辑（示例仅广播事件，实际项目在此接入场景/状态切换）。
    /// </summary>
    protected override Task OnExecuteAsync()
    {
        this.SendEvent(new GameStartedEvent());
        return Task.CompletedTask;
    }
}
