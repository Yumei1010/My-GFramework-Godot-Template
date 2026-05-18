using GFramework.Core.extensions;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using Godot;
using TimeToTwentyfour.scripts.cqrs.poker.command;
using TimeToTwentyfour.scripts.enums.poker;

namespace TimeToTwentyfour.scripts.entities.poker.state;

/// <summary>
///     扑克空闲状态，等待玩家交互输入以切换到选中或拖拽状态。
/// </summary>
[Log]
[ContextAware]
public partial class IdleState : PokerState
{
    public override void Enter()
    {
        Poker.Reset("position");
    }

    public override void MouseDown()
    {
        ChangeTo(PokerStateType.Drag);
    }

    public override void MouseEnter()
    {
        this.SendCommand(new PokerUpdateViewScaleCommand { PokerId = Poker.Id, TargetScale = new Vector2(1.1f, 1.1f) });
    }

    public override void MouseExit()
    {
        this.SendCommand(new PokerResetViewScaleCommand { PokerId = Poker.Id });
    }
}
