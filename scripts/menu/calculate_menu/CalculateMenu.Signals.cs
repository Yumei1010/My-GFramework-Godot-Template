using GFramework.Core.extensions;
using TimeToTwentyfour.scripts.cqrs.deck.@event;

namespace TimeToTwentyfour.scripts.menu.calculate_menu;

public partial class CalculateMenu
{
    private void ConnectSignal()
    {
        CheckButton.ButtonDown += OnCheckButtonButtonDown;
        DiscardButton.ButtonDown += OnDiscardButtonButtonDown;
        SortBySuitButton.ButtonDown += OnSortBySuitButtonButtonDown;
        SortByRankButton.ButtonDown += OnSortByRankButtonButtonDown;
    }

    private void OnCheckButtonButtonDown()
    {
        this.SendEvent(new DeckHandCheckedEvent
        {
            Hands = Selector.Selects
        });
    }

    private void OnDiscardButtonButtonDown()
    {
        this.SendEvent(new DeckDiscardCheckedEvent
        {
            Hands = Selector.Selects
        });
    }

    private void OnSortBySuitButtonButtonDown()
    {
        Deck.SortBySuit();
    }

    private void OnSortByRankButtonButtonDown()
    {
        Deck.SortByRank();
    }
}
