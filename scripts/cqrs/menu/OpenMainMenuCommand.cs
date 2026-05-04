using GFramework.Core.Abstractions.state;
using GFramework.Core.command;
using GFramework.Core.extensions;
using TimeToTwentyfour.scripts.core.state.impls;

namespace TimeToTwentyfour.scripts.cqrs.menu.command;

/// <summary>
///     打开主菜单的命令类，用于将状态机切换到主菜单状态。
/// </summary>
public sealed class OpenMainMenuCommand : AbstractCommand
{
    /// <summary>
    ///     执行打开主菜单命令的具体逻辑，将当前状态切换为 MainMenuState。
    /// </summary>
    protected override void OnExecute()
    {
        this.GetSystem<IStateMachineSystem>()!.ChangeTo<MainMenuState>();
    }
}