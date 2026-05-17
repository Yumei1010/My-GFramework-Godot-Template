using GFramework.Core.command;
using GFramework.Core.extensions;
using TimeToTwentyfour.scripts.cqrs.deck.@event;
using TimeToTwentyfour.scripts.model.deck;

namespace TimeToTwentyfour.scripts.cqrs.deck.command;

public sealed class DeckAddPokerCommand : AbstractCommand
{
    public Guid PokerId { get; set; }

    protected override void OnExecute()
    {
        this.GetModel<DeckModel>().Pokers.Add(PokerId);

        this.SendEvent(new DeckPokerAddedEvent{ PokerId = PokerId }); 
    }
}