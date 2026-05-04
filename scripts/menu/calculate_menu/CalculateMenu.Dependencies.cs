using TimeToTwentyfour.global;
using TimeToTwentyfour.scripts.component.pokerFactory;
using TimeToTwentyfour.scripts.enums.poker;
using Godot;
using TimeToTwentyfour.scripts.component.calculator;
using TimeToTwentyfour.scripts.component.deck;
using TimeToTwentyfour.scripts.component.selector;
using TimeToTwentyfour.scripts.component.timeBar;

namespace TimeToTwentyfour.scripts.menu.calculate_menu;

public partial class CalculateMenu
{
    private ICalculator Calculator => GetNode<ICalculator>("%Calculator");
    private ISelector Selector => GetNode<ISelector>("%Selector");
    private IPokerFactory PokerFactory => GetNode<IPokerFactory>("%PokerFactory");
    private ITimeBar TimeBar => GetNode<ITimeBar>("%TimeBar");
    private IDeck Deck => GetNode<IDeck>("%Deck");
    private Button SelectButton => GetNode<Button>("%SelectButton");
    private Button CheckButton => GetNode<Button>("%CheckButton");
    private Button DiscardButton => GetNode<Button>("%DiscardButton");
    private Button SortBySuitButton => GetNode<Button>("%SortBySuitButton");
    private Button SortByRankButton => GetNode<Button>("%SortByRankButton");

    private async Task ReadyAsync()
    {
        await GameEntryPoint.Architecture.WaitUntilReadyAsync().ConfigureAwait(false);
        
        // TODO: 替换为正式的发牌/关卡系统
        CreateTest();
    }

    private void CreateTest()
    {
        TimeBar.Start(120f);
        TimeBar.TimeScale = 1f;
        
        AddCard(SuitType.Heart, "20");
        AddCard(SuitType.Diamond, "4");
        AddCard(SuitType.Spade, "6");
        AddCard(SuitType.Club, "8");
    }

    private void AddCard(SuitType suit, string value)
    {
        var poker = PokerFactory.Product();
        poker.SuitType = suit;
        poker.NumValue = value;
        Deck.Add(poker);
    }
}
