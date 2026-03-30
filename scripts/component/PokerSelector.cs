using GFramework.Core.extensions;
using GFramework.Godot.extensions;
using GFramework.SourceGenerators.Abstractions.rule;
using GFrameworkGodotTemplate.scripts.enums.poker;
using GFrameworkGodotTemplate.scripts.events.pokerSelector;
using GFrameworkGodotTemplate.scripts.poker;
using Godot;

namespace GFrameworkGodotTemplate.scripts.component;

[ContextAware]
public partial class PokerSelector : Node
{
    private string Mode { get; set; } = null!;  
    private IList<Poker> Selects { get; set; } = new List<Poker>();
    private bool Enable { get; set; }
    
    public override void _Ready()
    {
        RegisterEvent();
    }

    private void RegisterEvent()
    {
        // 注册对可用性变更事件的监听
        this.RegisterEvent<EnableChangedEvent>(e =>
        {
            OnEnableChangedEvent(e.Enable);
        }).UnRegisterWhenNodeExitTree(this);
        
        // 注册对选择变更事件的监听
        this.RegisterEvent<SelectChangedEvent>(e =>
        {
            OnSelectChangedEvent(e.Poker);
        }).UnRegisterWhenNodeExitTree(this);
        
        // 注册对选择变更事件的监听
        this.RegisterEvent<ModeChangedEvent>(e =>
        {
            OnModeChangedEvent(e.Mode);
        }).UnRegisterWhenNodeExitTree(this);
    }

    private void OnEnableChangedEvent(bool enable)
    {
        Enable = enable;
    }

    private void OnSelectChangedEvent(Poker poker)
    {
        // 如果选择器不可用，返回，否则，更新选择对象
        if (!Enable) return;
            
        // 如果卡牌拥有状态未选择，添加进选择对象中
        if (poker.States.Contains(StateType.UnSelect))
        {
            // 如果选择对象中拥有该poker，返回
            if (Selects.Contains(poker)) return;
                
            Selects.Add(poker);
            poker.States.Remove(StateType.UnSelect);
            poker.States.Add(StateType.OnSelect);
            return;
        }
            
        // 如果卡牌拥有状态选择中，从选择对象中移除
        if (poker.States.Contains(StateType.OnSelect))
        {
            // 如果选择对象未中拥有该poker，返回
            if (!Selects.Contains(poker)) return;
                
            Selects.Remove(poker);
            poker.States.Remove(StateType.OnSelect);
            poker.States.Add(StateType.UnSelect);
        }
            
        Calculate();
    }

    private void OnModeChangedEvent(String mode)
    {
        Mode = mode;
        
        // 清除选择对象
        if (Selects.Count != 0)
        {
            Selects.Clear();
        }
    }
    
    private void Calculate()
    {
        // 如果选择对象为空，返回
        if (Selects.Count == 0) return;
        
        // 如果选择对象已满
        if (Selects.Count == 2)
        {
            
        }
    }
}