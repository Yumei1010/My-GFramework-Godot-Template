using GFramework.Core.command;
using GFramework.Core.extensions;
using GFrameworkGodotTemplate.scripts.model;
using Godot;

namespace GFrameworkGodotTemplate.scripts.command.poker;

public class StartMoveCommand : AbstractCommand
{
    protected override void OnExecute()
    {
        var model = this.GetModel<IPokerModel>()!;
        model.IsMoving = true;
        Input.SetMouseMode(Input.MouseModeEnum.ConfinedHidden);
    }
}