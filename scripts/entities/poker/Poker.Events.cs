using GFramework.Core.extensions;
using GFramework.Godot.extensions;
using GFrameworkGodotTemplate.scripts.cqrs.poker.@event;
using GFrameworkGodotTemplate.scripts.enums.poker;
using GFrameworkGodotTemplate.scripts.enums.resources;
using Godot;

namespace GFrameworkGodotTemplate.scripts.entities.poker;

public partial class Poker
{
    private void RegisterEvent()
    {
        // 注册对花色变更事件的监听
        ContextAwareExtensions.RegisterEvent<PokerSuitTypeChangedEvent>(this, e =>
        {
            OnSuitTypeChangedEvent(e.SuitType,e.Poker);
        }).UnRegisterWhenNodeExitTree(this);
        
        // 注册对数值变更事件的监听
        ContextAwareExtensions.RegisterEvent<PokerNumValueChangedEvent>(this, e =>
        {
            OnNumValueChangedEvent(e.NumValue,e.Poker);
        }).UnRegisterWhenNodeExitTree(this);
        
        // 注册对数值类型变更事件的监听
        ContextAwareExtensions.RegisterEvent<PokerNumTypeChangedEvent>(this, e =>
        {
            OnNumTypeChangedEvent(e.NumType,e.Poker);
        }).UnRegisterWhenNodeExitTree(this);
        
        // 注册对状态变更事件的监听
        ContextAwareExtensions.RegisterEvent<PokerStateChangedEvent>(this, e =>
        {
            OnStateChangedEvent(e.State,e.Poker);
        }).UnRegisterWhenNodeExitTree(this);
        
        // 注册对选择器可用性变更事件的监听
        ContextAwareExtensions.RegisterEvent<PokerSelectorEnableChangedEvent>(this, e =>
        {
            OnEnableChangedEvent(e.Enable);
        }).UnRegisterWhenNodeExitTree(this);
        
        // 注册对预览运算结果变更事件的监听
        ContextAwareExtensions.RegisterEvent<PokerReserveResultChangedEvent>(this, e =>
        {
            OnReserveResultChangedEvent(e.NumValue, e.IsHidden, e.Poker);
        }).UnRegisterWhenNodeExitTree(this);
    }
    
    private void OnSuitTypeChangedEvent(SuitType suitType,IPoker poker)
    {
        // 如果不是触发事件的poker，返回
        if (poker != this) return;
        // 新值与旧值相等，返回
        if (SuitType == suitType) return;
        
        // 更新花色和贴图
        SuitType = suitType;
        SurfaceRect.Texture = suitType switch
        {
            SuitType.Heart => _textureRegistry.Get(nameof(TextureKey.PokerSuitHeart)) as Texture2D,
            SuitType.Diamond => _textureRegistry.Get(nameof(TextureKey.PokerSuitDiamond)) as Texture2D,
            SuitType.Spade => _textureRegistry.Get(nameof(TextureKey.PokerSuitSpade)) as Texture2D,
            SuitType.Club => _textureRegistry.Get(nameof(TextureKey.PokerSuitClub)) as Texture2D,
            _ => null
        };
    }
    
    private void OnNumValueChangedEvent(String numValue,IPoker poker)
    {
        // 如果不是触发事件的poker，返回
        if (poker != this) return;
        
        // 为null，返回
        if (numValue == null!) return;
        
        // 新值与旧值相等，返回
        if (string.Equals(NumValue, numValue, StringComparison.Ordinal)) return;
        
        // 更新数值和显示
        NumValue = numValue;
        NumLabel.Text = numValue;
    }
    
    private void OnNumTypeChangedEvent(NumType numType,IPoker poker)
    {
        // 如果不是触发事件的poker，返回
        if (poker != this) return;
        
        // 新值与旧值相等，返回
        if (NumType == numType) return;
        
        // 更新数值类型
        NumType = numType;
    }
    
    private void OnStateChangedEvent(StateType stateType,IPoker poker)
    {
        // 如果不是触发事件的poker，返回
        if (poker != this) return;
        
        StateMachine.ChangeTo(stateType);
    }
    
    private void OnEnableChangedEvent(bool enable)
    {
        StateMachine.ChangeTo(enable ? StateType.UnSelect : StateType.Idle);
    }

    private void OnReserveResultChangedEvent(String numValue,bool visible,IPoker poker)
    {
        // 如果不是触发事件的poker，返回
        if (poker != this) return;

        ReserveResultRect.Visible = visible;
        
        ReserveResultLabel.Text = numValue;
    }
}