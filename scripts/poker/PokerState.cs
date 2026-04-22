using GFramework.Core.extensions;
using GFramework.SourceGenerators.Abstractions.rule;
using GFrameworkGodotTemplate.scripts.enums.poker;
using GFrameworkGodotTemplate.scripts.events.pokerStateMachine;
using Godot;

namespace GFrameworkGodotTemplate.scripts.poker;

/// <summary>
///     扑克状态抽象基类
/// </summary>
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
    /// <param name="targetStateType">目标状态 <see cref="StateType"/></param>
    protected void ChangeTo(StateType targetStateType)
    {
        this.SendEvent(new PokerStateMachineStateChangedEvent
        {
            TargetState = targetStateType,
            State = this
        });
    }
}