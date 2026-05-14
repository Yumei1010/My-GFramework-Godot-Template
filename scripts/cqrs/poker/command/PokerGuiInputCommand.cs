using GFramework.Core.command;
using GFramework.Core.extensions;
using Godot;
using TimeToTwentyfour.scripts.system.Poker;

namespace TimeToTwentyfour.scripts.cqrs.poker.command;

public sealed class PokerGuiInputCommand : AbstractCommand
{
    public Guid PokerId {get; init; }
    public InputEvent InputEvent {get; init; }

    protected override void OnExecute()
    {
        this.GetSystem<PokerStateSystem>().GuiInput(PokerId, InputEvent);
    }
}