using Godot;
using TimeToTwentyfour.scripts.enums.poker;

namespace TimeToTwentyfour.scripts.entities.poker.state;

/// <summary>
///     扑克选中状态，表示该牌已被选择器标记为选中。
/// </summary>
public sealed partial class OnSelectState : PokerState
{
    public override void Enter()
    {
        Vector2 pos = Poker.GlobalPosition;
        pos.Y -= Poker.Size.Y / 2;
        Poker.GlobalPosition = pos;
    }

    public override void Exit()
    {
        Vector2 pos = Poker.GlobalPosition;
        pos.Y += Poker.Size.Y / 2;
        Poker.GlobalPosition = pos;
    }

    public override void MouseDown()
    {
        ChangeTo(StateType.UnSelect);
    }
}
