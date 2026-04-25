using GFramework.Core.extensions;
using TimeToTwentyfour.scripts.cqrs.poker.@event;

namespace TimeToTwentyfour.scripts.entities.poker;

public partial class Deck
{
    private void ConnectSignal()
    {
        HolderContainer.SortChildren += OnHolderContainerSortChildren;
    }

    private void OnHolderContainerSortChildren()
    {
        this.SendEvent(new DeckSortFinishedEvent());
    }
}