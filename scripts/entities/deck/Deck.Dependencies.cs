using TimeToTwentyfour.global;
using Godot;
using TimeToTwentyfour.scripts.entities.selector;

namespace TimeToTwentyfour.scripts.entities.deck;

public partial class Deck
{
    private ISelector Selector => GetNode<ISelector>("%Selector");
    private Control PokerContainer => GetNode<Control>("%PokerContainer");
    private HBoxContainer HolderContainer => GetNode<HBoxContainer>("%HolderContainer");
    private Button CheckButton => GetNode<Button>("%CheckButton");
    private Button DiscardButton => GetNode<Button>("%DiscardButton");
    private Button SortBySuitButton => GetNode<Button>("%SortBySuitButton");
    private Button SortByRankButton => GetNode<Button>("%SortByRankButton");
    
    private async Task ReadyAsync()
    {
        // 等待框架加载完成
        await GameEntryPoint.Architecture.WaitUntilReadyAsync().ConfigureAwait(false);
    }
}