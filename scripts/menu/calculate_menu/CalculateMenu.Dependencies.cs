using GFramework.Core.extensions;
using TimeToTwentyfour.global;
using TimeToTwentyfour.scripts.component.pokerFactory;
using TimeToTwentyfour.scripts.enums.poker;
using Godot;
using TimeToTwentyfour.scripts.component.calculator;
using TimeToTwentyfour.scripts.component.deck;
using TimeToTwentyfour.scripts.component.selector;
using TimeToTwentyfour.scripts.component.timeBar;
using TimeToTwentyfour.scripts.cqrs.pile.command;
using TimeToTwentyfour.scripts.model.pileModel;

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
        DealTest();
    }

    /// <summary>简易发牌测试：从抽牌堆随机取 4 张牌并加入牌桌。</summary>
    private void DealTest()
    {
        TimeBar.Start(120f);
        TimeBar.TimeScale = 1f;

        var handPile = this.GetModel<HandPileModel>();

        for (int i = 0; i < 4; i++)
        {
            this.SendCommand(new DrawPileDrawToHandCommand());
            var card = handPile.Pile.Last();
            Deck.Add(PokerFactory.Product(card));
        }
    }
}
