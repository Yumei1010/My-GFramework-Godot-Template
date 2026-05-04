namespace TimeToTwentyfour.scripts.component.deck;

public partial class Deck
{
    private void ConnectSignal()
    {
        HolderContainer.SortChildren += OnHolderContainerSortChildren;
    }

    private void OnHolderContainerSortChildren()
    {
        ReLayout();
    }
}
