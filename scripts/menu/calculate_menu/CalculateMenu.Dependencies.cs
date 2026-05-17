using GFramework.Core.extensions;
using TimeToTwentyfour.global;
using TimeToTwentyfour.scripts.entities.deck;
using TimeToTwentyfour.scripts.system.poker;
using TimeToTwentyfour.scripts.entities.time_bar;
using TimeToTwentyfour.scripts.cqrs.pile.command;
using TimeToTwentyfour.scripts.model.pile;

namespace TimeToTwentyfour.scripts.menu.calculate_menu;

public partial class CalculateMenu
{
    private PokerManager PokerFactory => GetNode<PokerManager>("/root/PokerManager");
    private ITimeBar TimeBar => GetNode<ITimeBar>("%TimeBar");
    private IDeck Deck => GetNode<IDeck>("%Deck");

    private async Task ReadyAsync()
    {
        await GameEntryPoint.Architecture.WaitUntilReadyAsync().ConfigureAwait(false);

        // TODO: 替换为正式的发牌/关卡系统
        DealTest();
    }

    /// <summary>简易发牌测试：从抽牌堆随机取 4 张牌并加入牌桌。</summary>
    private void DealTest()
    {
        var handPile = this.GetModel<HandPileModel>();

        for (int i = 0; i < 4; i++)
        {
            this.SendCommand(new DrawPileDrawToHandCommand());
            var card = handPile.Pile.Last();
            Deck.Add(PokerFactory.Product(card));
        }
    }
}
