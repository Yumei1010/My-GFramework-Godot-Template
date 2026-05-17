using GFramework.Core.extensions;
using GFramework.Godot.extensions;
using TimeToTwentyfour.scripts.cqrs.mode_button.@event;
using TimeToTwentyfour.scripts.entities.calculator.mode;
using TimeToTwentyfour.scripts.entities.poker;

namespace TimeToTwentyfour.scripts.entities.calculator;

/// <summary>
///     <see cref="Calculator"/> 的 CQRS 事件订阅文件。
/// </summary>
public partial class Calculator
{
    private void RegisterEvent()
    {
        this.RegisterEvent<ModeButtonClickedEvent>(e =>
        {
            ChangeTo(e.ModeType);
        }).UnRegisterWhenNodeExitTree(this);
    }

    /// <summary>
    ///     根据当前模式和手牌列表执行计算验证与分发。
    /// </summary>
    /// <returns>计算结果字符串，或错误码。</returns>
    internal static string Evaluate(IMode? mode, IReadOnlyList<IPokerData> hands)
    {
        if (mode == null!)
            return "ERROR:NoModeSelected";

        if (mode.IsBinary)
        {
            if (hands.Count < 2)
                return "ERROR:InsufficientHands";
            return mode.Calculate(hands[0], hands[1]);
        }
        else
        {
            if (hands.Count < 1)
                return "ERROR:InsufficientHands";
            return mode.Calculate(hands[0]);
        }
    }
}