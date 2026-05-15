using TimeToTwentyfour.scripts.enums.poker;

namespace TimeToTwentyfour.scripts.entities.poker.state;

/// <summary>
///     扑克空闲状态，等待玩家交互输入以切换到选中或拖拽状态。
/// </summary>
public sealed class IdleState : PokerState
{
    public override void Enter()
    {
        Poker.Reset("position");
    }

    public override void MouseDown()
    {
        ChangeTo(StateType.Drag);
    }
}
