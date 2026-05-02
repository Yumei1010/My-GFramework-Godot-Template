using GFramework.Core.extensions;
using TimeToTwentyfour.scripts.cqrs.deck.@event;

namespace TimeToTwentyfour.scripts.entities.deck;

public partial class Deck
{
    private void ConnectSignal()
    {
        HolderContainer.SortChildren += OnHolderContainerSortChildren;
        CheckButton.ButtonDown += OnCheckButtonButtonDown;
        DiscardButton.ButtonDown += OnDiscardButtonButtonDown;
        SortBySuitButton.ButtonDown += OnSortBySuitButtonButtonDown;
        SortByRankButton.ButtonDown += OnSortByRankButtonButtonDown;
    }

    private void OnHolderContainerSortChildren()
    {
        ReLayout();
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
        SortBySuit();
    }
    
    private void OnSortByRankButtonButtonDown()
    {
        SortByRank();
    }
}