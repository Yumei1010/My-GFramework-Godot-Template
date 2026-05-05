using GFramework.Core.extensions;
using GFramework.SourceGenerators.Abstractions.rule;
using TimeToTwentyfour.scripts.cqrs.selector.@event;
using TimeToTwentyfour.scripts.enums.poker;

namespace TimeToTwentyfour.scripts.entities.poker.state;

/// <summary>
///     扑克取消选中状态，从选中状态回退到空闲前的过渡状态。
/// </summary>
[ContextAware]
public sealed partial class UnSelectState : PokerState
{
    public override void MouseDown()
    {
        this.SendEvent(new SelectorSelectChangedEvent
        {
            Poker = Poker,
            IsSelected = true
        });

        ChangeTo(StateType.OnSelect);
    }

    public override void MouseEnter()
    {
        this.SendEvent(new PokerSelectorReservesChangedEvent
        {
            Poker = Poker,
            IsSelected = true
        });
    }

    public override void MouseExit()
    {
        this.SendEvent(new PokerSelectorReservesChangedEvent
        {
            Poker = Poker,
            IsSelected = false
        });
    }
}
