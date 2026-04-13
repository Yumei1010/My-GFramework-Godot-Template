using GFramework.Core.extensions;
using GFramework.Godot.extensions;
using GFramework.SourceGenerators.Abstractions.rule;
using GFrameworkGodotTemplate.scripts.events.pokerSelector;
using GFrameworkGodotTemplate.scripts.poker;
using Godot;

namespace GFrameworkGodotTemplate.scripts.component;

[ContextAware]
public partial class PokerSelector : Node
{
    private string Mode { get; set; } = null!;  
    private IList<IPoker> Selects { get; set; } = new List<IPoker>();
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
            OnSelectChangedEvent(e.Poker,e.IsSelected);
        }).UnRegisterWhenNodeExitTree(this);
        
        // 注册对模式变更事件的监听
        this.RegisterEvent<ModeChangedEvent>(e =>
        {
            OnModeChangedEvent(e.Mode);
        }).UnRegisterWhenNodeExitTree(this);
    }
    
    private void OnEnableChangedEvent(bool enable)
    {
        Enable = enable;
    }
    
    private void OnSelectChangedEvent(IPoker poker,bool isSelected)
    {
        if (Enable)
        {
            if (isSelected)
            {
                if (Selects.Count == 2)
                {
                    Selects.RemoveAt(0);
                }
                
                Selects.Add(poker);
            }
            else
            {
                Selects.Remove(poker);
            }
        }
    }
    
    private void OnModeChangedEvent(String mode)
    {
        Mode = mode;
    }
    
    private void Calculate()
    {
        // 如果选择对象为空，返回
        if (Selects.Count == 0) return;
        
        // 如果选择对象已满
        if (Selects.Count == 2)
        {
            this.SendEvent(new CapacityChangedEvent
            {
                IsFull = true,
                Loads = Selects
            });
        }
    }
}