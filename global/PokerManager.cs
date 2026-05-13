using GFramework.Core.Abstractions.enums;
using GFramework.Core.Abstractions.system;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using Godot;
using TimeToTwentyfour.scripts.entities.poker;
using TimeToTwentyfour.scripts.entities.poker.state;
using TimeToTwentyfour.scripts.enums.poker;
using TimeToTwentyfour.scripts.model.pile;

namespace TimeToTwentyfour.global;

[Log]
[ContextAware]
public partial class PokerManager : Node, ISystem
{
    [Export] private PackedScene _pokerScene = GD.Load<PackedScene>("res://scenes/component/poker_view/poker_view.tscn");

    public IPoker Product()
    {
        return _pokerScene.Instantiate<IPoker>();
    }

    public IPoker Product(SuitType suitType, string numValue, NumType numType = NumType.Integer)
    {
        var poker = Product();
        poker.SuitType = suitType;
        poker.NumValue = numValue;
        poker.NumType = numType;
        return poker;
    }

    public IPoker Product(Card card)
    {
        var poker = Product();
        ((IPokerView)poker).Id = card.Id;
        poker.SuitType = card.SuitType;
        poker.NumValue = card.NumValue;
        poker.NumType = card.NumType;
        return poker;
    }

    private readonly Dictionary<Guid, IPokerView> _map = new();

    public void Register(Guid id, IPokerView poker) => _map[id] = poker;

    public void Unregister(Guid id) => _map.Remove(id);

    public IPokerView? Find(Guid id) => _map.GetValueOrDefault(id);

    private readonly Dictionary<Guid, StateBundle> _bundles = new();

    private struct StateBundle
    {
        public Dictionary<StateType, IPokerState> States;
        public IPokerState CurrentState;
        public IPokerState? PreviousState;
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

        _bundles[poker.Id] = new StateBundle { States = states };
    }

    public void RemoveBundle(Guid id) => _bundles.Remove(id);

    public void ChangeTo(Guid id, StateType stateType)
    {
        var b = _bundles[id];

        if (b.CurrentState == null!)
        {
            b.CurrentState = b.States[stateType];
            b.CurrentState.Enter();
            _bundles[id] = b;
            return;
        }

        if (b.CurrentState == b.States[stateType]) return;

        b.CurrentState.Exit();
        b.PreviousState = b.CurrentState;
        b.CurrentState = b.States[stateType];
        b.CurrentState.Enter();
        _bundles[id] = b;
    }

    public void Process(Guid id, double delta)
    {
        _bundles[id].CurrentState.Process(delta);
    }

    public void GuiInput(Guid id, InputEvent inputEvent)
    {
        _bundles[id].CurrentState.GuiInput(inputEvent);
    }

    public void MouseDown(Guid id)
    {
        _bundles[id].CurrentState.MouseDown();
    }

    public void MouseUp(Guid id)
    {
        _bundles[id].CurrentState.MouseUp();
    }

    public void MouseEnter(Guid id)
    {
        _bundles[id].CurrentState.MouseEnter();
    }

    public void MouseExit(Guid id)
    {
        _bundles[id].CurrentState.MouseExit();
    }

    public void OnArchitecturePhase(ArchitecturePhase phase)
    {
        
    }

    public void Init()
    {
        
    }

    public void Destroy()
    {
        
    }
}
