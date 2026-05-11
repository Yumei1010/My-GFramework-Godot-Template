using System;
using GFramework.Core.command;
using GFramework.Core.extensions;
using TimeToTwentyfour.scripts.model.pile;

namespace TimeToTwentyfour.scripts.cqrs.pile.command;

/// <summary>
///    将弃牌堆洗牌放入抽牌堆的命令
/// </summary>
public sealed class DiscardPileShuffleToDrawPileCommand : AbstractCommand
{
    protected override void OnExecute()
    {
        IList<Card> Pile = [];
        Pile = this.GetModel<DiscardPileModel>().Pile;
        this.GetModel<DrawPileModel>().Pile = Pile;
        this.GetModel<DiscardPileModel>().Pile = [];
    }
}
