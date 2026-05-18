using GFramework.Core.command;
using GFramework.Core.extensions;
using TimeToTwentyfour.scripts.system.deck;

namespace TimeToTwentyfour.scripts.cqrs.deck.command;

public sealed class DeckRemovePokerCommand : AbstractCommand
{
    public required Guid PokerId { get; set; }

    protected override void OnExecute()
    {
        this.GetSystem<DeckSystem>().DiscardCard(PokerId);
    }
}
