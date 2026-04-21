using GFramework.Core.extensions;
using GFramework.Godot.extensions;
using GFramework.SourceGenerators.Abstractions.rule;
using GFrameworkGodotTemplate.scripts.enums.poker;
using GFrameworkGodotTemplate.scripts.events.pokerStateMachine;
using Godot;

namespace GFrameworkGodotTemplate.scripts.poker;

[ContextAware]
public partial class PokerStateMachine : Node, IPokerStateMachine
{
    private IPokerState PreviousState { get; set; } = null!;
    private IPokerState CurrentState { get; set; } = null!;
    private Dictionary<StateType, IPokerState> States { get; set; } = new();

    public override void _Ready()
    {
        RegisterEvent();
    }

    private void RegisterEvent()
    {
        this.RegisterEvent<PokerStateMachineStateChangedEvent>(e =>
        {
            OnStateChangedEvent(e.TargetState,e.State);
        }).UnRegisterWhenNodeExitTree(this);
    }
    
    public void Init(IPoker poker)
    {
        foreach (var node in GetChildren())
        {
            var state = (IPokerState)node;
            States.Add(state.GetStateType(), state);
            state.SetPoker(poker);
        }
    }

    public void ChangeTo(StateType stateType)
    {
        // 如果首次设置状态，方法变更为设置当前状态为目标状态
        if (CurrentState == null!)
        {
            CurrentState = States[stateType];
            CurrentState.Enter();
            return;
        }
        
        // 如果目标状态与当前状态相同，返回
        if (CurrentState == States[stateType]) return;
        
        // 退出当前状态
        CurrentState.Exit();
        // 先前状态赋值为当前状态
        PreviousState = CurrentState;
        // 当前状态赋值为目标状态
        CurrentState = States[stateType];
        // 进入目标状态
        CurrentState.Enter();
    }
    
    public void Process(double delta)
    {
        if (CurrentState != null!)
        {
            CurrentState.Process(delta);
        }
    }
    
    public void MouseDown()
    {
        if (CurrentState != null!)
        {
            CurrentState.MouseDown();
        }
    }

    public void MouseUp()
    {
        if (CurrentState != null!)
        {
            CurrentState.MouseUp();
        }
    }

    public void MouseEnter()
    {
        if (CurrentState != null!)
        {
            CurrentState.MouseEnter();
        }
    }

    public void MouseExit()
    {
        if (CurrentState != null!)
        {
            CurrentState.MouseExit();
        }
    }

    private void OnStateChangedEvent(StateType stateType,IPokerState state)
    {
        // 如果不是触发事件的state，返回
        if (!States.ContainsValue(state)) return;
        
        ChangeTo(stateType);
    }
}