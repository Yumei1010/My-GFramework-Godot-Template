using GFramework.Core.extensions;
using TimeToTwentyfour.scripts.cqrs.deck.@event;

namespace TimeToTwentyfour.scripts.menu.calculate_menu;

public partial class CalculateMenu
{
    private void ConnectSignal()
    {
        CheckButton.ButtonDown += OnButtonDownCheckButton;
        DiscardButton.ButtonDown += OnButtonDownDiscardButton;
        SortBySuitButton.ButtonDown += OnButtonDownSortBySuitButton;
        SortByRankButton.ButtonDown += OnButtonDownSortByRankButton;
        AddButton.ButtonDown += OnButtonDownAddButton;
        SubtractButton.ButtonDown += OnButtonDownSubtractButton;
        MultiplyButton.ButtonDown += OnButtonDownMultiplyButton; 
        DivideButton.ButtonDown += OnButtonDownDivideButton;
        ModuloButton.ButtonDown += OnButtonDownModuloButton;
        NthRootButton.ButtonDown += OnButtonDownNthRootButton;
        PowerButton.ButtonDown += OnButtonDownPowerButton;
        AbsoluteValueButton.ButtonDown += OnButtonDownAbsoluteValueButton;
        FactorialButton.ButtonDown += OnButtonDownFactorialButton;
        SquareRootButton.ButtonDown += OnButtonDownSquareRootButton;
        CeilButton.ButtonDown += OnButtonDownCeilButton;
        FloorButton.ButtonDown += OnButtonDownFloorButton;
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
    
    private void OnButtonDownNthRootButton()
    {

    }

    private void OnButtonDownAddButton()
    {

    }

    private void OnButtonDownSubtractButton()
    {

    }

    private void OnButtonDownMultiplyButton()
    {

    }

    private void OnButtonDownDivideButton()
    {

    }

    private void OnButtonDownModuloButton()
    {

    }

    private void OnButtonDownSquareRootButton()
    {

    }

    private void OnButtonDownCeilButton()
    {

    }

    private void OnButtonDownFloorButton()
    {

    }

    private void OnButtonDownPowerButton()
    {

    }
    
    private void OnButtonDownAbsoluteValueButton()
    {

    }

    private void OnButtonDownFactorialButton()
    {

    }
}
