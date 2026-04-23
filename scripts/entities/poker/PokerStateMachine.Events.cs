using GFramework.Core.extensions;
using GFramework.Godot.extensions;
using GFrameworkGodotTemplate.scripts.cqrs.poker.@event;
using GFrameworkGodotTemplate.scripts.enums.poker;

namespace GFrameworkGodotTemplate.scripts.entities.poker;

public partial class PokerStateMachine
{
    private void RegisterEvent()
    {
        // 注册对状态变更事件的监听
        this.RegisterEvent<PokerStateMachineStateChangedEvent>(e =>
        {
            OnStateChangedEvent(e.TargetState,e.State);
        }).UnRegisterWhenNodeExitTree(this);
    }
    
    private void OnStateChangedEvent(StateType stateType,IPokerState state)
    {
        // 如果不是触发事件的state，返回
        if (!States.ContainsValue(state)) return;
        
        ChangeTo(stateType);
    }
}