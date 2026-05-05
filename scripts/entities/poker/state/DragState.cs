using GFramework.Core.extensions;
using GFramework.SourceGenerators.Abstractions.rule;
using Godot;
using TimeToTwentyfour.scripts.cqrs.poker.@event;
using TimeToTwentyfour.scripts.enums.poker;

namespace TimeToTwentyfour.scripts.entities.poker.state;

/// <summary>
///     扑克拖拽状态，处理鼠标拖拽时的位置跟随与释放逻辑。
/// </summary>
[ContextAware]
public sealed partial class DragState : PokerState
{
    public override void Process(double delta)
    {
        Poker.GlobalPosition = Poker.GetGlobalMousePosition() - Poker.Size / 2;
    }

    public override void Enter()
    {
        this.SendEvent(new PokerDragStartedEvent
        {
            Poker = Poker
        });

        Input.SetMouseMode(Input.MouseModeEnum.ConfinedHidden);

        Poker.TopLevel = true;
    }

    public override void Exit()
    {
        this.SendEvent(new PokerDragFinishedEvent
        {
            Poker = Poker
        });

        Input.SetMouseMode(Input.MouseModeEnum.Visible);

        Poker.TopLevel = false;
    }

    public override void MouseUp()
    {
        ChangeTo(StateType.Idle);
    }
}
