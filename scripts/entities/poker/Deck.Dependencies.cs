using TimeToTwentyfour.global;
using Godot;

namespace TimeToTwentyfour.scripts.entities.poker;

public partial class Deck
{
    private HBoxContainer HolderContainer => GetNode<HBoxContainer>("%HolderContainer");
    
    private async Task ReadyAsync()  
    {
        // 等待框架加载完成
        await GameEntryPoint.Architecture.WaitUntilReadyAsync().ConfigureAwait(false);
        
        // 初始化卡套组
        Holders = HolderContainer.GetChildren().Cast<IPokerHolder>().ToList();
    }
}