using GFramework.Core.command;
using GFramework.Core.extensions;
using Godot;

namespace GFrameworkGodotTemplate.scripts.command.menu.select;

public class SelectNextPileCommand : AbstractCommand
{
    protected override void OnExecute()
    {
        GD.Print("SelectNextPileCommand.OnExecute()");
    }
}