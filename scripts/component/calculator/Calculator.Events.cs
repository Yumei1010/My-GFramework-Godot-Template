using GFramework.Core.extensions;
using GFramework.Godot.extensions;
using TimeToTwentyfour.scripts.cqrs.calculator.@event;
using TimeToTwentyfour.scripts.cqrs.deck.@event;
using TimeToTwentyfour.scripts.entities.poker;

namespace TimeToTwentyfour.scripts.component.calculator;

public partial class Calculator
{
    private void RegisterEvent()
    {
        this.RegisterEvent<DeckHandCheckedEvent>(e =>
        {
            OnDeckHandCheckedEvent(e.Hands);
        }).UnRegisterWhenNodeExitTree(this);
    }

    /// <summary>处理出牌确认事件：根据当前 Mode 类型执行二元或一元计算，并发送 <see cref="CalculatorResultEvent"/>。</summary>
    private void OnDeckHandCheckedEvent(IReadOnlyList<IPoker> hands)
    {
        if (CurrentMode == null!)
        {
            this.SendEvent(new CalculatorResultEvent
            {
                Result = "ERROR:NoModeSelected",
                Hands = hands,
                ModeType = null
            });
            return;
        }

        string result;
        if (CurrentMode.IsBinary)
        {
            if (hands.Count < 2)
            {
                this.SendEvent(new CalculatorResultEvent
                {
                    Result = "ERROR:InsufficientHands",
                    Hands = hands,
                    ModeType = CurrentMode.ModeType
                });
                return;
            }
            result = Calculate(hands[0], hands[1]);
        }
        else
        {
            if (hands.Count < 1)
            {
                this.SendEvent(new CalculatorResultEvent
                {
                    Result = "ERROR:InsufficientHands",
                    Hands = hands,
                    ModeType = CurrentMode.ModeType
                });
                return;
            }
            result = Calculate(hands[0]);
        }

        this.SendEvent(new CalculatorResultEvent
        {
            Result = result,
            Hands = hands,
            ModeType = CurrentMode.ModeType
        });
    }
}