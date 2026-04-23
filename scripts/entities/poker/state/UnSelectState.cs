using GFramework.Core.extensions;
using GFramework.SourceGenerators.Abstractions.rule;
using GFrameworkGodotTemplate.scripts.cqrs.poker.@event;
using GFrameworkGodotTemplate.scripts.enums.poker;

namespace GFrameworkGodotTemplate.scripts.entities.poker.state;

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