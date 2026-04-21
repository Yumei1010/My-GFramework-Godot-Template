using GFramework.Core.extensions;
using GFramework.SourceGenerators.Abstractions.rule;
using GFrameworkGodotTemplate.scripts.enums.poker;
using GFrameworkGodotTemplate.scripts.events.pokerSelector;

namespace GFrameworkGodotTemplate.scripts.poker.state;

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
        this.SendEvent(new PokerSelectorSelectChangedEvent
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

    }
    
    public override void MouseExit()
    {
        
    }
}