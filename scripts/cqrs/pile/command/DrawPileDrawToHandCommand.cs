using GFramework.Core.command;
using GFramework.Core.extensions;
using TimeToTwentyfour.scripts.model.pile;

namespace TimeToTwentyfour.scripts.cqrs.pile.command;

/// <summary>
///     从抽牌堆中抽牌并添加到手牌堆的命令
///     若抽牌堆为空，先触发弃牌堆洗回再抽。
/// </summary>
public sealed class DrawPileDrawToHandCommand : AbstractCommand
{
    protected override void OnExecute()
    {
        var drawPile = this.GetModel<DrawPileModel>();
        if (drawPile.Pile.Count == 0)
            this.SendCommand(new DiscardPileShuffleToDrawPileCommand());

        var card = drawPile.GetRandomCard();
        drawPile.RemoveCard(card);
        this.GetModel<HandPileModel>().AddCard(card);
    }
}
