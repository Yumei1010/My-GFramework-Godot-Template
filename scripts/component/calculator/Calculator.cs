using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using Godot;
using TimeToTwentyfour.scripts.enums.calculator;
using TimeToTwentyfour.scripts.entities.poker;

namespace TimeToTwentyfour.scripts.component.calculator;

[Log]
[ContextAware]
public partial class Calculator : Node, ICalculator
{
    public override void _Ready()
    {
        _ = ReadyAsync();
        RegisterEvent();
    }

    /// <summary>切换到指定运算模式。</summary>
    public void ChangeTo(ModeType modeType)
    {
        if (CurrentMode == null!)
        {
            CurrentMode = Modes[modeType];
            return;
        }

        if (CurrentMode == Modes[modeType]) return;

        PreviousMode = CurrentMode;
        CurrentMode = Modes[modeType];
    }
    
    public string Calculate(IPokerData pokerA, IPokerData pokerB)
    {
        return CurrentMode.Calculate(pokerA, pokerB);
    }
    
    public string Calculate(IPokerData poker)
    {
        return CurrentMode.Calculate(poker);
    }
}
