using System.Threading.Tasks;
using GFramework.Core.Command;
using GFrameworkTemplate.scripts.utility;

namespace GFrameworkTemplate.scripts.cqrs.game.command;

/// <summary>
///     退出游戏命令。
/// </summary>
/// <remarks>
///     Godot 主线程禁止同步 CQRS 调用（框架 GuardSyncCqrs），因此使用异步命令形式。
/// </remarks>
public sealed class ExitGameCommand : AbstractAsyncCommand
{
    /// <summary>
    ///     执行退出游戏逻辑。
    /// </summary>
    protected override Task OnExecuteAsync()
    {
        GameUtil.GetTree().Quit();
        return Task.CompletedTask;
    }
}
