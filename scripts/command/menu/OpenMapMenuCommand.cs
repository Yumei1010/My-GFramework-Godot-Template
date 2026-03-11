using GFramework.Core.Abstractions.state;
using GFramework.Core.command;
using GFramework.Core.extensions;
using GFrameworkGodotTemplate.scripts.core.state.impls;

namespace GFrameworkGodotTemplate.scripts.command.menu;

public class OpenMapMenuCommand : AbstractCommand
{
    protected override void OnExecute()
    {
        this.GetSystem<IStateMachineSystem>()!.ChangeTo<MapMenuState>();
    }
}