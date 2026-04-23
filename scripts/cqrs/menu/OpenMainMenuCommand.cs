using GFramework.Core.Abstractions.state;
using GFramework.Core.command;
using GFramework.Core.extensions;
using TimeToTwentyfour.scripts.core.state.impls;

namespace TimeToTwentyfour.scripts.cqrs.menu;

public class OpenMainMenuCommand : AbstractCommand
{
    protected override void OnExecute()
    {
        this.GetSystem<IStateMachineSystem>()!.ChangeTo<MainMenuState>();
    }
}