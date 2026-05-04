using TimeToTwentyfour.global;
using TimeToTwentyfour.scripts.component.pokerFactory;
using TimeToTwentyfour.scripts.entities.poker;
using TimeToTwentyfour.scripts.enums.poker;
using Godot;
using TimeToTwentyfour.scripts.component.calculator;
using TimeToTwentyfour.scripts.component.deck;
using TimeToTwentyfour.scripts.component.selector;
using TimeToTwentyfour.scripts.component.timeBar;

namespace TimeToTwentyfour.scripts.menu.calculate_menu;

public partial class CalculateMenu
{
    private ICalculator Calculator => GetNode<ICalculator>("Calculator");
    private ISelector Selector => GetNode<ISelector>("%Selector");
    private IPokerFactory PokerFactory => GetNode<IPokerFactory>("%PokerFactory");
    private ITimeBar TimeBar => GetNode<ITimeBar>("%TimeBar");
    private IDeck Deck => GetNode<IDeck>("%Deck");
    private Button CheckButton => GetNode<Button>("%CheckButton");
    private Button DiscardButton => GetNode<Button>("%DiscardButton");
    private Button SortBySuitButton => GetNode<Button>("%SortBySuitButton");
    private Button SortByRankButton => GetNode<Button>("%SortByRankButton");
    private TextureButton AddButton => GetNode<TextureButton>("%AddButton");
    private TextureButton SubtractButton => GetNode<TextureButton>("%SubtractButton");
    private TextureButton MultiplyButton => GetNode<TextureButton>("%MultiplyButton");
    private TextureButton DivideButton => GetNode<TextureButton>("%DivideButton");
    private TextureButton ModuloButton => GetNode<TextureButton>("%ModuloButton");
    private TextureButton NthRootButton => GetNode<TextureButton>("%NthRootButton");
    private TextureButton PowerButton => GetNode<TextureButton>("%PowerButton");
    private TextureButton AbsoluteValueButton => GetNode<TextureButton>("%AbsoluteValueButton");
    private TextureButton FactorialButton => GetNode<TextureButton>("%FactorialButton");
    private TextureButton SquareRootButton => GetNode<TextureButton>("%SquareRootButton");
    private TextureButton CeilButton => GetNode<TextureButton>("%CeilButton"); 
    private TextureButton FloorButton => GetNode<TextureButton>("%FloorButton");

    private async Task ReadyAsync()
    {
        await GameEntryPoint.Architecture.WaitUntilReadyAsync().ConfigureAwait(false);

        // 临时测试脚手架
        TimeBar.Start(120f);
        TimeBar.TimeScale = 1f;
        
        for (int i = 0; i < 1; i++)
        {
            IPoker poker = PokerFactory.Product();
            poker.SuitType = SuitType.Heart;
            poker.NumValue = "20";
            Deck.Add(poker);
        }

        for (int i = 0; i < 1; i++)
        {
            IPoker poker = PokerFactory.Product();
            poker.SuitType = SuitType.Diamond;
            poker.NumValue = "4";
            Deck.Add(poker);
        }

        for (int i = 0; i < 1; i++)
        {
            IPoker poker = PokerFactory.Product();
            poker.SuitType = SuitType.Spade;
            poker.NumValue = "6";
            Deck.Add(poker);
        }

        for (int i = 0; i < 1; i++)
        {
            IPoker poker = PokerFactory.Product();
            poker.SuitType = SuitType.Club;
            poker.NumValue = "8";
            Deck.Add(poker);
        }
    }
}
