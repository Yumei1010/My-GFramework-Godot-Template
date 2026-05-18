using GFramework.Core.command;
using GFramework.Core.extensions;
using TimeToTwentyfour.scripts.cqrs.deck.@event;
using TimeToTwentyfour.scripts.system.deck;

namespace TimeToTwentyfour.scripts.cqrs.deck.command
{
    public sealed class DeckSortCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            this.GetSystem<DeckSortSystem>().Sort();

            this.SendEvent(new DeckSortStartedEvent());
        }
    }
}