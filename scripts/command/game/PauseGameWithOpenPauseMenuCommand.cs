using GFramework.Core.command;
using GFramework.Core.extensions;
using GFramework.Game.Abstractions.ui;
using GFrameworkGodotTemplate.scripts.command.menu;
using GFrameworkGodotTemplate.scripts.command.menu.input;

namespace GFrameworkGodotTemplate.scripts.command.game;

/// <summary>
///     暂停游戏并打开暂停菜单的命令类。
///     该类继承自 AbstractCommand，并在执行时发送两个子命令：
///     1. 暂停游戏命令（PauseGameCommand）
///     2. 打开选项菜单命令（OpenOptionsMenuCommand）
/// </summary>
public class PauseGameWithOpenPauseMenuCommand(OpenPauseMenuCommandInput input) : AbstractCommand<OpenPauseMenuCommandInput, UiHandle>(input)
{
    /// <summary>
    ///     执行暂停游戏并打开暂停菜单的核心逻辑。
    ///     该方法会依次发送两个命令：
    ///     1. 发送暂停游戏命令，传入当前输入参数。
    ///     2. 发送打开选项菜单命令，无需额外参数。
    /// </summary>
    protected override UiHandle OnExecute(OpenPauseMenuCommandInput input)
    {
        // 发送暂停游戏命令
        this.SendCommand(new PauseGameCommand());

        // 发送打开选项菜单命令
        return this.SendCommand(new OpenPauseMenuCommand(input));
    }
}