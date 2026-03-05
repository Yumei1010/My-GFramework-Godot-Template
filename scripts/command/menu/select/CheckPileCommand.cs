using GFramework.Core.Abstractions.state;
using GFramework.Core.command;
using GFramework.Core.extensions;
using GFrameworkGodotTemplate.scripts.core.state.impls;

namespace GFrameworkGodotTemplate.scripts.command.menu.select;

public class CheckPileCommand : AbstractCommand
{
    protected override void OnExecute()
    {
        this.GetSystem<IStateMachineSystem>()!.ChangeTo<ClockMenuState>();
    }
}