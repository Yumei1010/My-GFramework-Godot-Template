using GFramework.Core.command;
using GFramework.Core.extensions;
using TimeToTwentyfour.scripts.cqrs.poker.@event;
using TimeToTwentyfour.scripts.enums.poker;
using TimeToTwentyfour.scripts.model.pileModel;

namespace TimeToTwentyfour.scripts.cqrs.poker.command;

/// <summary>
///     修改卡牌模型数据的命令
///     遍历抽牌堆、手牌堆、弃牌堆，通过 Id 查找 <see cref="Card"/> 并更新，随后发送 <see cref="CardChangedEvent"/> 驱动 Poker 视图刷新。
/// </summary>
public sealed class PokerChangeCardCommand : AbstractCommand
{
    public required Guid Id { get; init; }
    public required SuitType SuitType { get; init; }
    public required string NumValue { get; init; }
    public required NumType NumType { get; init; }

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
            pile.AddCard(new Card(Id, SuitType, NumValue, NumType));

            this.SendEvent(new PokerCardChangedEvent
            {
                Id = Id,
                SuitType = SuitType,
                NumValue = NumValue,
                NumType = NumType
            });
            return;
        }
    }
}
