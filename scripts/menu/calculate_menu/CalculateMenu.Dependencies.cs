using GFramework.Core.extensions;
using TimeToTwentyfour.global;
using TimeToTwentyfour.scripts.entities.deck;
using TimeToTwentyfour.scripts.entities.time_bar;
using TimeToTwentyfour.scripts.cqrs.deck.command;
using TimeToTwentyfour.scripts.cqrs.pile.command;
using TimeToTwentyfour.scripts.model.pile;

namespace TimeToTwentyfour.scripts.menu.calculate_menu;

public partial class CalculateMenu
{
    private ITimeBar TimeBar => GetNode<ITimeBar>("%TimeBar");
    private IDeck Deck => GetNode<IDeck>("%Deck");

    private async Task ReadyAsync()
    {
        await GameEntryPoint.Architecture.WaitUntilReadyAsync().ConfigureAwait(false);

        // TODO: 替换为正式的发牌/关卡系统
        DealTest();
    }

    /// <summary>简易发牌测试：通过 CQRS 命令链完成抽牌→发牌。</summary>
    private void DealTest()
    {
        for (int i = 0; i < 4; i++)
        {
            this.SendCommand(new DrawPileDrawToHandCommand());
            var card = this.GetModel<HandPileModel>().Pile.Last();
            this.SendCommand(new DeckAddPokerCommand { Card = card });
        }
    }
}
