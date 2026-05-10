using GFramework.Core.command;
using GFramework.Core.extensions;
using TimeToTwentyfour.scripts.cqrs.poker.@event;
using TimeToTwentyfour.scripts.enums.poker;
using TimeToTwentyfour.scripts.model.pileModel;

namespace TimeToTwentyfour.scripts.cqrs.poker.command;

/// <summary>
///     修改卡牌模型点数的命令
///     遍历三个牌堆，通过 Id 查找 <see cref="Card"/> 并更新点数，NumType 将自动推断。
/// </summary>
public sealed class PokerChangeCardValueCommand : AbstractCommand
{
    public required Guid Id { get; init; }
    public required string NumValue { get; init; }

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

            var numType = NumValue.Contains('/') ? NumType.Fraction
                : NumValue.Contains('.') ? NumType.Decimal
                : NumType.Integer;

            pile.RemoveCard(card);
            pile.AddCard(card with { NumValue = NumValue, NumType = numType });

            this.SendEvent(new PokerCardChangedEvent
            {
                Id = Id,
                SuitType = card.SuitType,
                NumValue = NumValue,
                NumType = numType
            });
            return;
        }
    }
}
