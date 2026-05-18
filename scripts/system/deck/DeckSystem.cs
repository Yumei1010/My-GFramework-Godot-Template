using GFramework.Core.Abstractions.enums;
using GFramework.Core.Abstractions.system;
using GFramework.Core.extensions;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using TimeToTwentyfour.scripts.cqrs.deck.@event;
using TimeToTwentyfour.scripts.model.deck;
using TimeToTwentyfour.scripts.model.pile;
using TimeToTwentyfour.scripts.system.poker;

namespace TimeToTwentyfour.scripts.system.deck;

[Log]
[ContextAware]
public partial class DeckSystem : ISystem
{
    public void OnArchitecturePhase(ArchitecturePhase phase)
    {
        _log.Debug("System initialized: DeckSystem");
    }

    public void Init()
    {
        
    }

    public void Destroy()
    {
        
    }

    /// <summary>
    ///     发牌：从手牌堆取牌数据，创建扑克节点并注册，更新牌桌模型，发送新增事件。
    /// </summary>
    public void DealCard(Card card)
    {
        var pokerManager = this.GetSystem<PokerSystem>();
        var poker = pokerManager.Product(card);
        pokerManager.Register(card.Id, poker);

        this.GetModel<DeckModel>().Pokers.Add(card.Id);
        this.SendEvent(new DeckPokerAddedEvent { PokerId = card.Id });
    }

    /// <summary>
    ///     弃牌：从牌桌模型移除，发送移除事件（视觉层响应后回收节点）。
    /// </summary>
    public void DiscardCard(Guid pokerId)
    {
        this.GetModel<DeckModel>().Pokers.Remove(pokerId);
        this.SendEvent(new DeckPokerRemovedEvent { PokerId = pokerId });
    }
}
