using TimeToTwentyfour.global;
using Godot;

namespace TimeToTwentyfour.scripts.entities.deck;

public partial class Deck
{
    private Control PokerContainer => GetNode<Control>("%PokerContainer");
    private HBoxContainer HolderContainer => GetNode<HBoxContainer>("%HolderContainer");
    
    private async Task ReadyAsync()
    {
        // 等待框架加载完成
        await GameEntryPoint.Architecture.WaitUntilReadyAsync().ConfigureAwait(false);
    }
}