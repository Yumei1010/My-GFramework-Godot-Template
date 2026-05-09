using GFramework.Core.command;
using GFramework.Core.extensions;
using TimeToTwentyfour.scripts.cqrs.poker.@event;
using TimeToTwentyfour.scripts.enums.poker;
using TimeToTwentyfour.scripts.model.pileModel;

namespace TimeToTwentyfour.scripts.cqrs.poker.command;

/// <summary>
///     修改卡牌模型花色的命令
///     遍历三个牌堆，通过 Id 查找 <see cref="Card"/> 并更新花色。
/// </summary>
public sealed class PokerChangeCardSuitCommand : AbstractCommand
{
    public required Guid Id { get; init; }
    public required SuitType SuitType { get; init; }

    protected override void OnExecute()
    {
        var piles = new PileModel[]
        {
            this.GetModel<DrawPileModel>(),
            this.GetModel<HandPileModel>(),
            this.GetModel<DiscardPileModel>()
        };

        foreach (var pile in piles)
        {
            var card = pile.Pile.FirstOrDefault(c => c.Id == Id);
            if (card == null)
                continue;

            pile.RemoveCard(card);
            pile.AddCard(card with { SuitType = SuitType });

            this.SendEvent(new CardChangedEvent
            {
                Id = Id,
                SuitType = SuitType,
                NumValue = card.NumValue,
                NumType = card.NumType
            });
            return;
        }
    }
}
