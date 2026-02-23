using GFramework.Core.command;
using GFramework.Core.extensions;
using Godot;

namespace GFrameworkGodotTemplate.scripts.command.menu.select;

public class SelectPrevPileCommand : AbstractCommand
{
    protected override void OnExecute()
    {
        GD.Print("SelectPrevPileCommand.OnExecute()");
    }
}