using TimeToTwentyfour.global;
using TimeToTwentyfour.scripts.component.pokerFactory;
using TimeToTwentyfour.scripts.entities.deck;
using TimeToTwentyfour.scripts.entities.poker;
using TimeToTwentyfour.scripts.entities.timeBar;
using TimeToTwentyfour.scripts.enums.poker;

namespace TimeToTwentyfour.scripts.menu.calculate_menu;

public partial class CalculateMenu
{
    private IPokerFactory PokerFactory => GetNode<IPokerFactory>("%PokerFactory");
    private ITimeBar TimeBar => GetNode<ITimeBar>("%TimeBar");
    private IDeck Deck => GetNode<IDeck>("%Deck");
    
    private async Task ReadyAsync()
    {
        // 等待框架加载完成
        await GameEntryPoint.Architecture.WaitUntilReadyAsync().ConfigureAwait(false);
        
        TimeBar.Start(120f);
        TimeBar.TimeScale = 1f;
        
        for (int i = 0; i < 1; i++)
        {
            IPoker poker = PokerFactory.Product();
            poker.SuitType = SuitType.Heart;
            poker.NumValue = "20";
            Deck.Add(poker);
        }
        
        for (int i = 0; i < 1; i++)
        {
            IPoker poker = PokerFactory.Product();
            poker.SuitType = SuitType.Diamond;
            poker.NumValue = "4";
            Deck.Add(poker);
        }
        
        for (int i = 0; i < 1; i++)
        {
            IPoker poker = PokerFactory.Product();
            poker.SuitType = SuitType.Spade;
            poker.NumValue = "6";
            Deck.Add(poker);
        }
        
        for (int i = 0; i < 1; i++)
        {
            IPoker poker = PokerFactory.Product();
            poker.SuitType = SuitType.Club;
            poker.NumValue = "8";
            Deck.Add(poker);
        }
    }
}