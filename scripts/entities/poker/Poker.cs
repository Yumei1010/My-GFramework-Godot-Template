using GFramework.Core.Abstractions.controller;
using GFramework.Core.extensions;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using Godot;
using TimeToTwentyfour.scripts.cqrs.poker.command;
using TimeToTwentyfour.scripts.enums.poker;

namespace TimeToTwentyfour.scripts.entities.poker;

[Log]
[ContextAware]
public partial class Poker : Button, IPoker, IController
{
    public override void _Ready()
    {
        _ = ReadyAsync();
        ConnectSignal();
        RegisterEvent();
    }

    public override void _ExitTree()
    {
        this.SendCommand(new PokerRemoveStateBundleCommand { PokerId = Id });
        this.SendCommand(new PokerRemoveAnimationBundleCommand { PokerId = Id });
        this.SendCommand(new PokerRemoveThemeBundleCommand { PokerId = Id });
    }

    public override void _Process(double delta)
    {
        this.SendCommand(new PokerUpdateShadowPositionCommand { PokerId = Id });
        this.SendCommand(new PokerProcessUpdateCommand { PokerId = Id, Delta = delta });
    }

    public override void _GuiInput(InputEvent @event)
    {
        this.SendCommand(new PokerUpdateFake3DRotationCommand { PokerId = Id });
        this.SendCommand(new PokerGuiInputCommand { PokerId = Id, InputEvent = @event });
    }

    public void Reparent(Node parent)
    {
        base.Reparent(parent);
    }

    public void ChangeTo(PokerStateType state)
    {
        this.SendCommand(new PokerChangeStateCommand{ PokerId = Id, State = state });
    }

    public void Reset(string attributeName)
    {
        switch (attributeName)
        {
            case "position":
                this.SendCommand(new PokerResetViewPositionCommand{ PokerId = Id , ResetPosition = ResetPosition });
                break;
            case "rotation":

            break;
        }
    }

}