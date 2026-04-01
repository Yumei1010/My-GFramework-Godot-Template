using GFramework.Core.extensions;
using GFramework.SourceGenerators.Abstractions.rule;
using GFrameworkGodotTemplate.scripts.enums.poker;
using GFrameworkGodotTemplate.scripts.events.poker;
using Godot;

namespace GFrameworkGodotTemplate.scripts.poker.state;

[ContextAware]
public partial class IdleState : Node, IPokerState
{
    private IPoker Poker { get; set; } = null!;
    
    public void SetPoker(IPoker poker)
    {
        Poker = poker;
    }
    
    public void Process(double delta)
    {
        
    }

    public void Enter()
    {
        _ = Poker.ResetPosAndRot();
    }

    public void Exit()
    {

    }

    public void MouseDown()
    {
        this.SendEvent(new StateChangedEvent()
        {
            CurrentState = StateType.Idle,
            NextState = StateType.Drag
        });
    }

    public void MouseUp()
    {
        
    }

    public void MouseEnter()
    {
        
    }
    
    public void MouseExit()
    {
        
    }
}