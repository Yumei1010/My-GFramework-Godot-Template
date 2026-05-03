using TimeToTwentyfour.scripts.entities.poker.state;
using TimeToTwentyfour.scripts.enums.poker;

namespace TimeToTwentyfour.scripts.entities.poker.stateMachine;

public partial class PokerStateMachine
{
    /// <summary>
    ///     状态表 <see cref="Dictionary{StateType, IPokerState}" />
    /// </summary>
    private Dictionary<StateType, IPokerState> States { get; } = new();
    
    /// <summary>
    ///     先前状态 <see cref="IPokerState" />
    /// </summary>
    private IPokerState PreviousState { get; set; } = null!;

    /// <summary>
    ///     当前状态 <see cref="IPokerState" />
    /// </summary>
    private IPokerState CurrentState { get; set; } = null!;
}