using GFramework.Core.Abstractions.enums;
using GFramework.Core.Abstractions.system;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using Godot;
using TimeToTwentyfour.scripts.entities.poker;
using TimeToTwentyfour.scripts.entities.poker.state;
using TimeToTwentyfour.scripts.enums.poker;

namespace TimeToTwentyfour.scripts.system.Poker;

[Log]
[ContextAware]
public partial class PokerStateSystem : ISystem
{
    private struct StateBundle
    {
        public Dictionary<StateType, IPokerState> States;
        public IPokerState CurrentState;
        public IPokerState PreviousState;
    }

    private Dictionary<Guid, StateBundle> Bundles = [];

    public void OnArchitecturePhase(ArchitecturePhase phase)
    {
        _log.Debug("System initialized: PokerStateSystem");
    }
    
    public void Init()
    {
        
    }

    public void Destroy()
    {
        
    }

    public void InitStates(IPokerView poker)
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
            state.StateType = type;
            state.Poker = poker;
        }

        Bundles[poker.Id] = new StateBundle { States = states };
    }

    public void RemoveBundle(Guid id) => Bundles.Remove(id);

    public void ChangeTo(Guid id, StateType state)
    {
        var b = Bundles[id];

        if (b.CurrentState == null!)
        {
            b.CurrentState = b.States[state];
            b.CurrentState.Enter();
            Bundles[id] = b;
            return;
        }

        if (b.CurrentState == b.States[state]) return;

        b.CurrentState.Exit();
        b.PreviousState = b.CurrentState;
        b.CurrentState = b.States[state];
        b.CurrentState.Enter();
        Bundles[id] = b;
    }

    public void Process(Guid id, double delta)
    {
        Bundles[id].CurrentState.Process(delta);
    }

    public void GuiInput(Guid id, InputEvent inputEvent)
    {
        Bundles[id].CurrentState.GuiInput(inputEvent);
    }

    public void MouseDown(Guid id)
    {
        Bundles[id].CurrentState.MouseDown();
    }

    public void MouseUp(Guid id)
    {
        Bundles[id].CurrentState.MouseUp();
    }

    public void MouseEnter(Guid id)
    {
        Bundles[id].CurrentState.MouseEnter();
    }

    public void MouseExit(Guid id)
    {
        Bundles[id].CurrentState.MouseExit();
    }
}
