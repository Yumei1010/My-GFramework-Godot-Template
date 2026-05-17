using GFramework.Core.command;
using GFramework.Core.extensions;
using TimeToTwentyfour.scripts.cqrs.deck.@event;
using TimeToTwentyfour.scripts.enums.deck;
using TimeToTwentyfour.scripts.model.deck;

namespace TimeToTwentyfour.scripts.cqrs.deck.command;

public sealed class DeckSortByValueCommand : AbstractCommand
{
    protected override void OnExecute()
    {
        this.GetModel<DeckModel>().CurrentSortMode = DeckSortMode.Value;

        this.SendEvent(new DeckSortStartedEvent());
    }
}