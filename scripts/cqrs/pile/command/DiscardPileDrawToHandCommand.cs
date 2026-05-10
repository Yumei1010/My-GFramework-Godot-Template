using System;
using GFramework.Core.command;
using GFramework.Core.extensions;
using TimeToTwentyfour.scripts.model.pileModel;

namespace TimeToTwentyfour.scripts.cqrs.pile.command;

/// <summary>
///    从弃牌堆中抽牌并添加到手牌堆的命令
/// </summary>
public sealed class DiscardPileDrawToHandCommand : AbstractCommand
{
    protected override void OnExecute()
    {
        var card = this.GetModel<DiscardPileModel>().GetRandomCard();
        this.GetModel<DiscardPileModel>().RemoveCard(card);
        this.GetModel<HandPileModel>().AddCard(card);
    }
}
