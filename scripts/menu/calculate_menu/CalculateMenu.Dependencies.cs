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

    /// <summary>等待框架就绪后进入临时测试发牌流程（TODO: 替换为 IRunManager 正式发牌）。</summary>
    private async Task ReadyAsync()
    {
        await GameEntryPoint.Architecture.WaitUntilReadyAsync().ConfigureAwait(false);

        // TODO: 替换为正式的发牌/关卡系统
        CreateTest();
    }

    /// <summary>临时测试脚手架：启动 120 秒计时器并发 4 张测试牌。</summary>
    private void CreateTest()
    {
        TimeBar.Start(120f);
        TimeBar.TimeScale = 1f;

        Deck.Add(PokerFactory.Product(SuitType.Heart, "20"));
        Deck.Add(PokerFactory.Product(SuitType.Diamond, "4"));
        Deck.Add(PokerFactory.Product(SuitType.Spade, "6"));
        Deck.Add(PokerFactory.Product(SuitType.Club, "8"));
    }
}
