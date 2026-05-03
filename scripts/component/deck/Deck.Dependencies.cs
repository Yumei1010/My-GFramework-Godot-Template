using global::TimeToTwentyfour.global;
using Godot;

namespace TimeToTwentyfour.scripts.component.deck;

public partial class Deck
{
    private Control PokerContainer => GetNode<Control>("%PokerContainer");
    private HBoxContainer HolderContainer => GetNode<HBoxContainer>("%HolderContainer");

    private async Task ReadyAsync()
    {
        await GameEntryPoint.Architecture.WaitUntilReadyAsync().ConfigureAwait(false);
    }
}
