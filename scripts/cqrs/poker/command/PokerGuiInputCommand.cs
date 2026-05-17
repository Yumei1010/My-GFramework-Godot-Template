using GFramework.Core.command;
using GFramework.Core.extensions;
using Godot;
using TimeToTwentyfour.scripts.system.poker;

namespace TimeToTwentyfour.scripts.cqrs.poker.command;

public sealed class PokerGuiInputCommand : AbstractCommand
{
    public required Guid PokerId {get; set; }
    public required InputEvent InputEvent {get; set; }

    protected override void OnExecute()
    {
        this.GetSystem<PokerStateSystem>().GuiInput(PokerId, InputEvent);
    }
}