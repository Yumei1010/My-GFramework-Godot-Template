using GFramework.Core.command;
using GFramework.Core.extensions;
using TimeToTwentyfour.scripts.cqrs.menu.command;
using TimeToTwentyfour.scripts.cqrs.menu.input;

namespace TimeToTwentyfour.scripts.cqrs.game.command;

/// <summary>
///     恢复游戏并关闭暂停菜单的命令类。
///     继承自 AbstractCommand，用于处理恢复游戏逻辑，并在执行时发送关闭暂停菜单和恢复游戏的命令。
/// </summary>
public sealed class ResumeGameWithClosePauseMenuCommand(ClosePauseMenuCommandInput input) : AbstractCommand<ClosePauseMenuCommandInput>(input)
{
    /// <summary>
    ///     执行命令的核心逻辑。
    ///     首先发送关闭暂停菜单的命令，然后发送恢复游戏的命令。
    /// </summary>
    protected override void OnExecute(ClosePauseMenuCommandInput input)
    {
        // 发送关闭暂停菜单的命令
        this.SendCommand(new ClosePauseMenuCommand(input));
        // 发送恢复游戏的命令
        this.SendCommand(new ResumeGameCommand());
    }
}