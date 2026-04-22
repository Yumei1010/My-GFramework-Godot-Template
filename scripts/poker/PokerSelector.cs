using GFramework.Core.extensions;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using GFrameworkGodotTemplate.scripts.enums.calculate;
using GFrameworkGodotTemplate.scripts.enums.poker;
using GFrameworkGodotTemplate.scripts.events.poker;
using Godot;

namespace GFrameworkGodotTemplate.scripts.poker;

[Log]
[ContextAware]
public partial class PokerSelector : Node , IPokerSelector
{
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
            var mode = (IPokerSelectorMode)node;
            Modes.Add(mode.GetModeType(), mode);
        }
    }

    public void Calculate()
    {
        // 如果选择对象和预览对象为空，返回
        if (Selects.Count == 0 && Reserves.Count == 0) return;

        // 如果选择对象数量满足模式要求
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
        
        // 清除选择对象和预览对象
        Clear();
    }

    public void Clear()
    {
        // 如果选择对象为空或预览对象为空，返回
        if (Selects.Count == 0 || Reserves.Count == 0) return;
        
        // 依次释放选择对象
        foreach (var poker in Selects)
        {
            this.SendEvent(new PokerStateChangedEvent
            {
                State = StateType.UnSelect,
                Poker = poker
            });
        }
        
        // 清空选择对象
        Selects.Clear();
        
        // 清空预览对象
        Reserves.Clear();
    }
}