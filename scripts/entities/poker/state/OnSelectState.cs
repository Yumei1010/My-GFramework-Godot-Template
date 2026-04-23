using GFramework.Core.extensions;
using GFramework.SourceGenerators.Abstractions.rule;
using TimeToTwentyfour.scripts.cqrs.poker.@event;
using TimeToTwentyfour.scripts.enums.poker;
using Godot;

namespace TimeToTwentyfour.scripts.entities.poker.state;

[ContextAware]
public partial class OnSelectState : PokerState
{
    public override void Process(double delta)
    {
        
    }

    public override void Enter()
    {
        Vector2 pos = Poker.GetGlobalPosition();
        pos.Y -= Poker.GetSize().Y / 2;   
        Poker.SetGlobalPosition(pos);
    }

    public override void Exit()
    {
        Vector2 pos = Poker.GetGlobalPosition();
        pos.Y += Poker.GetSize().Y / 2;   
        Poker.SetGlobalPosition(pos);
    }

    public override void MouseDown()
    {
        ContextAwareExtensions.SendEvent(this, new PokerSelectorSelectChangedEvent
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