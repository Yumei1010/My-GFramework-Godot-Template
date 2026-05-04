using GFramework.Core.extensions;
using TimeToTwentyfour.scripts.cqrs.deck.@event;
using TimeToTwentyfour.scripts.cqrs.selector.@event;
using TimeToTwentyfour.scripts.enums.calculator;

namespace TimeToTwentyfour.scripts.menu.calculate_menu;

public partial class CalculateMenu
{
    private void ConnectSignal()
    {
        SelectButton.ButtonDown += OnButtonDownSelectButton;
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

    private void OnButtonDownSelectButton()
    {
        if (Selector.Enable)
        {
            this.SendEvent(new SelectorEnableChangedEvent
            {
                Enable = false
            });
        }
        else
        {
            this.SendEvent(new SelectorEnableChangedEvent
            {
                Enable = true
            });
        }
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
        Calculator.ChangeTo(ModeType.NthRoot);
    }

    private void OnButtonDownAddButton()
    {
        Calculator.ChangeTo(ModeType.Add);
    }

    private void OnButtonDownSubtractButton()
    {
        Calculator.ChangeTo(ModeType.Subtract);
    }

    private void OnButtonDownMultiplyButton()
    {
        Calculator.ChangeTo(ModeType.Multiply);
    }

    private void OnButtonDownDivideButton()
    {
        Calculator.ChangeTo(ModeType.Divide);
    }

    private void OnButtonDownModuloButton()
    {
        Calculator.ChangeTo(ModeType.Modulo);
    }

    private void OnButtonDownSquareRootButton()
    {
        Calculator.ChangeTo(ModeType.SquareRoot);
    }

    private void OnButtonDownCeilButton()
    {
        Calculator.ChangeTo(ModeType.Ceil);
    }

    private void OnButtonDownFloorButton()
    {
        Calculator.ChangeTo(ModeType.Floor);
    }

    private void OnButtonDownPowerButton()
    {
        Calculator.ChangeTo(ModeType.Power);
    }
    
    private void OnButtonDownAbsoluteValueButton()
    {
        Calculator.ChangeTo(ModeType.AbsoluteValue);
    }

    private void OnButtonDownFactorialButton()
    {
        Calculator.ChangeTo(ModeType.Factorial);
    }
}
