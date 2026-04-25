using TimeToTwentyfour.global;
using Godot;
using TimeToTwentyfour.scripts.enums.poker;

namespace TimeToTwentyfour.scripts.entities.poker;

public partial class Deck
{
    private HBoxContainer HolderContainer => GetNode<HBoxContainer>("%HolderContainer");
    private IPokerFactory PokerFactory => GetNode<IPokerFactory>("%PokerFactory");
    private IPokerHolderFactory HolderFactory => GetNode<IPokerHolderFactory>("%HolderFactory");
    
    private async Task ReadyAsync()  
    {
        // 等待框架加载完成
        await GameEntryPoint.Architecture.WaitUntilReadyAsync().ConfigureAwait(false);


        for (int i = 0; i < 1; i++)
        {
            IPoker poker = PokerFactory.Product();   
            poker.SetSuitType(SuitType.Heart);
            Add(poker);
        }
        
        for (int i = 0; i < 1; i++)
        {
            IPoker poker = PokerFactory.Product();   
            poker.SetSuitType(SuitType.Diamond);
            Add(poker);
        }
        
        for (int i = 0; i < 1; i++)
        {
            IPoker poker = PokerFactory.Product();   
            poker.SetSuitType(SuitType.Spade);
            Add(poker);
        }
    }
}