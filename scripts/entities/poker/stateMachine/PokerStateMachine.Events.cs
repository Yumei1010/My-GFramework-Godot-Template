using GFramework.Core.extensions;
using GFramework.Godot.extensions;
using TimeToTwentyfour.scripts.cqrs.poker.@event;
using TimeToTwentyfour.scripts.entities.poker.state;
using TimeToTwentyfour.scripts.enums.poker;

namespace TimeToTwentyfour.scripts.entities.poker.stateMachine;

public partial class PokerStateMachine
{
    private void RegisterEvent()
    {
        // 注册对状态变更事件的监听
        ContextAwareExtensions.RegisterEvent<PokerStateChangedEvent>(this, e =>
        {
            OnStateChangedEvent(e.TargetState,e.State);
        }).UnRegisterWhenNodeExitTree(this);
    }
    
    private void OnStateChangedEvent(StateType stateType,IPokerState state)
    {
        // 如果不是己方持有的state，返回
        if (!States.ContainsValue(state)) return;
        
        ChangeTo(stateType);
    }
}