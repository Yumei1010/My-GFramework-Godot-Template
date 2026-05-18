namespace TimeToTwentyfour.scripts.entities.deck;

public partial class Deck
{
    private void ConnectSignal()
    {
        HolderContainer.SortChildren += OnHolderContainerSortChildren;
    }

    private void OnHolderContainerSortChildren()
    {
        
    }
}
