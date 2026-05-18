using GFramework.Core.command;
using GFramework.Core.extensions;
using TimeToTwentyfour.scripts.model.pile;
using TimeToTwentyfour.scripts.system.deck;

namespace TimeToTwentyfour.scripts.cqrs.deck.command;

public sealed class DeckAddPokerCommand : AbstractCommand
{
    public required Card Card { get; set; }

    protected override void OnExecute()
    {
        this.GetSystem<DeckSystem>().DealCard(Card);
    }
}
