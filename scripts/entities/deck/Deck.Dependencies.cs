using TimeToTwentyfour.global;
using Godot;
using TimeToTwentyfour.scripts.entities.poker;

namespace TimeToTwentyfour.scripts.entities.deck;

public partial class Deck
{
    private Control PokerContainer => GetNode<Control>("%PokerContainer");
    private HBoxContainer HolderContainer => GetNode<HBoxContainer>("%HolderContainer");
    private IPokerFactory PokerFactory => GetNode<IPokerFactory>("%PokerFactory");
    
    private async Task ReadyAsync()  
    {
        // 等待框架加载完成
        await GameEntryPoint.Architecture.WaitUntilReadyAsync().ConfigureAwait(false);

        for (int i = 0; i < 4; i++)
        {
            IPoker poker = PokerFactory.Product();
            
            PokerContainer.AddChild(poker as Node);

            Panel holder = new Panel();
            holder.Modulate = Holder ? new Color("ffffff2d") : new Color("ffffff00");
            holder.SizeFlagsHorizontal = SizeFlags.ExpandFill;
            
            HolderContainer.AddChild(holder);
        }
    }
}