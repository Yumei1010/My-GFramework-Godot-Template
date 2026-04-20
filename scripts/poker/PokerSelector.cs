using GFramework.Core.extensions;
using GFramework.Godot.extensions;
using GFramework.SourceGenerators.Abstractions.rule;
using GFrameworkGodotTemplate.scripts.enums.calculate;
using GFrameworkGodotTemplate.scripts.enums.poker;
using GFrameworkGodotTemplate.scripts.events.poker;
using GFrameworkGodotTemplate.scripts.events.pokerSelector;
using Godot;

namespace GFrameworkGodotTemplate.scripts.poker;

[ContextAware]
public partial class PokerSelector : Node , IPokerSelector
{
    private IPokerSelectorMode PreviousMode { get; set; } = null!;
    private IPokerSelectorMode CurrentMode { get; set; } = null!;
    private Dictionary<ModeType, PokerSelectorMode> Modes { get; set; } = new();
    private IList<IPoker> Selects { get; set; } = new List<IPoker>();
    private bool Enable { get; set; }
    
    public override void _Ready()
    {
        RegisterEvent();
        Init();
        ChangeTo(ModeType.Add);
    }
    
    public void Init()
    {
        foreach (var node in GetChildren())
        {
            var mode = (PokerSelectorMode)node;
            Modes.Add(mode.ModeType, mode);
        }
    }

    public void Calculate()
    {
        // 如果选择对象为空，返回
        if (Selects.Count == 0) return;
        
        // 如果选择对象已满足模式要求
        if (Selects.Count == CurrentMode.GetCapacity())
        {
            CurrentMode.SetPokers(Selects);
            CurrentMode.Calculate();
        }
    }

    public void ChangeTo(ModeType mode)
    {
        // 如果首次设置模式，方法变更为设置当前模式为目标模式
        if (CurrentMode == null!)
        {
            CurrentMode = Modes[mode];
            return;
        }
        
        // 如果目标模式与当前模式相同，返回
        if (CurrentMode == Modes[mode]) return;
        
        // 先前模式赋值为当前模式
        PreviousMode = CurrentMode;
        // 当前模式赋值为目标模式
        CurrentMode = Modes[mode];
        
        // 清除选择对象
        Clear();
    }

    public void Clear()
    {
        // 如果选择对象为空，返回
        if (Selects.Count == 0) return;
        
        // 依次释放选择对象
        foreach (var poker in Selects)
        {
            this.SendEvent(new PokerStateChangedEvent
            {
                State = StateType.UnSelect,
                Poker = poker
            });
        }
        
        // 清空选择对象列表
        Selects.Clear();
    }
    
    private void RegisterEvent()
    {
        // 注册对可用性变更事件的监听
        this.RegisterEvent<PokerSelectorEnableChangedEvent>(e =>
        {
            OnEnableChangedEvent(e.Enable);
        }).UnRegisterWhenNodeExitTree(this);
        
        // 注册对选择变更事件的监听
        this.RegisterEvent<PokerSelectorSelectChangedEvent>(e =>
        {
            OnSelectChangedEvent(e.Poker,e.IsSelected);
        }).UnRegisterWhenNodeExitTree(this);
        
        // 注册对模式变更事件的监听
        this.RegisterEvent<PokerSelectorModeChangedEvent>(e =>
        {
            OnModeChangedEvent(e.Mode);
        }).UnRegisterWhenNodeExitTree(this);
    }
    
    private void OnEnableChangedEvent(bool enable)
    {
        // 更新可用性
        Enable = enable;
        
        // 清除选择对象
        Clear();
    }
    
    private void OnSelectChangedEvent(IPoker poker,bool isSelected)
    {
        // 如果选择器未启用，返回
        if (!Enable) return;
        
        // 如果被选中，添加进选择对象，否则，移除
        if (isSelected)
        {
            // 如果选择对象中已经包含，返回
            if (Selects.Contains(poker)) return;
            
            // 如果选择对象已满，释放队首
            if (Selects.Count == CurrentMode.GetCapacity())
            {
                this.SendEvent(new PokerStateChangedEvent
                {
                    State = StateType.UnSelect,
                    Poker = Selects[0]
                });
                
                Selects.RemoveAt(0);
            }
                
            Selects.Add(poker);
        }
        else
        {
            // 如果选择对象中未包含，返回
            if (!Selects.Contains(poker)) return;
            
            Selects.Remove(poker);
        }
    }
    
    private void OnModeChangedEvent(ModeType mode)
    {
        ChangeTo(mode);
    }
}