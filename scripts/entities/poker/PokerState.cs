using GFramework.Core.extensions;
using GFramework.SourceGenerators.Abstractions.rule;
using TimeToTwentyfour.scripts.cqrs.poker.@event;
using TimeToTwentyfour.scripts.enums.poker;
using Godot;

namespace TimeToTwentyfour.scripts.entities.poker;

[ContextAware]
public abstract partial class PokerState : Node, IPokerState
{
    /// <summary>
    ///     扑克状态标识符 <see cref="StateType"/>
    /// </summary>
    [Export] protected StateType StateType { get; set; }
    
    /// <summary>
    ///     代理的扑克 <see cref="IPoker"/> 实例
    /// </summary>
    protected IPoker Poker { get; private set; } = null!;

    public abstract void GuiInput(InputEvent inputEvent);
    
    public abstract void MouseDown();
    
    public abstract void MouseUp();
    
    public abstract void MouseEnter();
    
    public abstract void MouseExit();

    public abstract void Process(double delta);

    public abstract void Enter();
    
    public abstract void Exit();

    public void SetPoker(IPoker poker)
    {
        Poker =  poker;
    }
    
    public StateType GetStateType()
    {
        return StateType;
    }
    
    /// <summary>
    ///     切换到指定状态
    /// </summary>
    /// <param name="toState">目标状态 <see cref="StateType"/></param>
    protected void ChangeTo(StateType toState)
    {
        this.SendEvent(new PokerStateMachineStateChangedEvent
        {
            TargetState = toState,
            State = this
        });
    }
}