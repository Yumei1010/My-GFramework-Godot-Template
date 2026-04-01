using GFramework.Core.extensions;
using GFramework.Godot.extensions;
using GFramework.SourceGenerators.Abstractions.rule;
using GFrameworkGodotTemplate.scripts.enums.poker;
using GFrameworkGodotTemplate.scripts.events.poker;
using GFrameworkGodotTemplate.scripts.poker.state;
using Godot.Collections;
using Godot;

namespace GFrameworkGodotTemplate.scripts.poker;

[ContextAware]
public partial class PokerStateMachine : Node, IPokerStateMachine
{
    public IPokerState PreviousState { get; set; } = null!;
    public IPokerState CurrentState { get; set; } = null!;
    public Dictionary States { get; set; } = null!;
    
    public override void _Ready()
    {
        RegisterEvent();
    }
    
    private void RegisterEvent()
    {
        // 注册对状态变更事件的监听
        this.RegisterEvent<StateChangedEvent>(e =>
        {
            OnStateChangedEvent(e.NextState);
        }).UnRegisterWhenNodeExitTree(this);
    }
    
    public void Process(double delta)
    {
        CurrentState.Process(delta);
    }

    public void SetInitState(IPokerState state)
    {
        CurrentState = state;
        CurrentState.Enter();
    }
    
    public void MouseDown()
    {
        CurrentState.MouseDown();
    }

    public void MouseUp()
    {
        CurrentState.MouseUp();
    }

    public void MouseEnter()
    {
        CurrentState.MouseEnter();
    }

    public void MouseExit()
    {
        CurrentState.MouseExit();
    }

    public void ChangeTo(IPokerState state)
    {
        state.SetPoker((IPoker)GetParent());
        
        // 如果新状态与当前状态相同，返回
        if (CurrentState == state) return;
        
        // 退出当前状态
        PreviousState = CurrentState;
        PreviousState.Exit();
        
        // 进入新状态
        CurrentState = state;
        CurrentState.Enter();
    }
    
    private void OnStateChangedEvent(StateType state)
    {
        IPokerState newState = state switch
        {
            StateType.Idle => new IdleState(),
            StateType.Drag => new DragState(),
            StateType.OnSelect => new OnSelectState(),
            StateType.UnSelect => new UnSelectState(),
            _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
        };
        
        ChangeTo(newState);
    }
}