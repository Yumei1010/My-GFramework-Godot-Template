using GFramework.Core.extensions;
using GFramework.SourceGenerators.Abstractions.rule;
using TimeToTwentyfour.scripts.enums.poker;
using Godot;
using TimeToTwentyfour.scripts.cqrs.selector.@event;

namespace TimeToTwentyfour.scripts.entities.poker.state;

/// <summary>
///     扑克选中状态，表示该牌已被选择器标记为选中。
/// </summary>
[ContextAware]
public partial class OnSelectState : PokerState
{
    public override void GuiInput(InputEvent inputEvent)
    {
        
    }
    
    public override void Process(double delta)
    {
        
    }

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
        this.SendEvent(new SelectorSelectChangedEvent
        {
            Poker = Poker,
            IsSelected = false
        });
        
        ChangeTo(StateType.UnSelect);
    }

    public override void MouseUp()
    {
        
    }

    public override void MouseEnter()
    {
        
    }
    
    public override void MouseExit()
    {
        
    }
}