using GFramework.Core.command;
using GFramework.Core.extensions;
using Godot;
using Twentyfour.scripts.model;

namespace Twentyfour.scripts.command.poker;

public class FinishMoveCommand : AbstractCommand
{
    protected override void OnExecute()
    {
        var model = this.GetModel<IPokerModel>()!;
        model.IsMoving = false;
        model.Movement();
        Input.SetMouseMode(Input.MouseModeEnum.Visible);
    }
}