using Godot;
using TimeToTwentyfour.scripts.entities.poker.state;
using TimeToTwentyfour.scripts.enums.poker;

namespace TimeToTwentyfour.scripts.entities.poker.stateMachine;

public partial class PokerStateMachine : Node, IPokerStateMachine
{
    public void Init(IPoker poker)
    {
        var states = new Dictionary<StateType, IPokerState>
        {
            { StateType.Idle, new IdleState() },
            { StateType.Drag, new DragState() },
            { StateType.OnSelect, new OnSelectState() },
            { StateType.UnSelect, new UnSelectState() },
        };

        foreach (var (type, state) in states)
        {
            States.Add(type, state);
            state.StateType = type;
            state.Poker = poker;
        }
    }

    public void ChangeTo(StateType stateType)
    {
        if (CurrentState == null!)
        {
            CurrentState = States[stateType];
            CurrentState.Enter();
            return;
        }

        if (CurrentState == States[stateType]) return;

        CurrentState.Exit();
        PreviousState = CurrentState;
        CurrentState = States[stateType];
        CurrentState.Enter();
    }

    public void GuiInput(InputEvent inputEvent)
    {
        CurrentState.GuiInput(inputEvent);
    }

    public void Process(double delta)
    {
        CurrentState.Process(delta);
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
}
