using GFramework.Core.extensions;
using TimeToTwentyfour.scripts.cqrs.deck.@event;
using TimeToTwentyfour.scripts.cqrs.selector.@event;
using Godot;

namespace TimeToTwentyfour.scripts.menu.calculate_menu;

public partial class CalculateMenu
{
    /// <summary>连接 Godot 按钮信号并桥接至 CQRS 事件。</summary>
    private void ConnectSignal()
    {
        SelectButton.ButtonDown += OnButtonDownSelectButton;
        CheckButton.ButtonDown += OnButtonDownCheckButton;
        DiscardButton.ButtonDown += OnButtonDownDiscardButton;
        SortBySuitButton.ButtonDown += OnButtonDownSortBySuitButton;
        SortByRankButton.ButtonDown += OnButtonDownSortByRankButton;
    }

    private void OnButtonDownSelectButton()
    {
        this.SendEvent(new SelectorEnableChangedEvent
        {
            Enable = !Selector.Enable
        });
    }
    
    private void OnButtonDownCheckButton()
    {
        this.SendEvent(new DeckHandCheckedEvent
        {
            Hands = Selector.Selects
        });
    }

    private void OnButtonDownDiscardButton()
    {
        this.SendEvent(new DeckDiscardCheckedEvent
        {
            Hands = Selector.Selects
        });
    }

    private void OnButtonDownSortBySuitButton()
    {
        Deck.SortBySuit();
    }

    private void OnButtonDownSortByRankButton()
    {
        Deck.SortByRank();
    }
}
