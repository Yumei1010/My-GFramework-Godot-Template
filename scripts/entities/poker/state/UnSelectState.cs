using GFramework.Core.extensions;
using GFramework.SourceGenerators.Abstractions.rule;
using TimeToTwentyfour.scripts.cqrs.poker.@event;
using TimeToTwentyfour.scripts.enums.poker;

namespace TimeToTwentyfour.scripts.entities.poker.state;

[ContextAware]
public partial class UnSelectState : PokerState
{
    public override void Process(double delta)
    {
        
    }

    public override void Enter()
    {
        
    }

    public override void Exit()
    {

    }

    public override void MouseDown()
    {
        ContextAwareExtensions.SendEvent(this, new PokerSelectorSelectChangedEvent
        {
            Poker = Poker,
            IsSelected = true
        });
        
        ChangeTo(StateType.OnSelect);
    }

    public override void MouseUp()
    {
        
    }

    public override void MouseEnter()
    {
        ContextAwareExtensions.SendEvent(this, new PokerSelectorReservesChangedEvent
        {
            Poker = Poker,
            IsSelected = true
        });
    }
    
    public override void MouseExit()
    {
        ContextAwareExtensions.SendEvent(this, new PokerSelectorReservesChangedEvent
        {
            Poker = Poker,
            IsSelected = false
        });
    }
}