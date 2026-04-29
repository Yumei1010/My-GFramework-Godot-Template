using GFramework.Core.extensions;
using TimeToTwentyfour.global;
using TimeToTwentyfour.scripts.component.pokerFactory;
using TimeToTwentyfour.scripts.cqrs.selector.@event;
using TimeToTwentyfour.scripts.entities.deck;
using TimeToTwentyfour.scripts.entities.poker;
using TimeToTwentyfour.scripts.entities.selector;
using TimeToTwentyfour.scripts.entities.timeBar;

namespace TimeToTwentyfour.scripts.menu.calculate_menu;

public partial class CalculateMenu
{
    private IPokerFactory PokerFactory => GetNode<IPokerFactory>("%PokerFactory");
    private ISelector Selector => GetNode<ISelector>("%Selector");
    private ITimeBar TimeBar => GetNode<ITimeBar>("%TimeBar");
    private IDeck Deck => GetNode<IDeck>("%Deck");
    
    private async Task ReadyAsync()
    {
        // 等待框架加载完成
        await GameEntryPoint.Architecture.WaitUntilReadyAsync().ConfigureAwait(false);
        
        TimeBar.Start(120f);
        TimeBar.TimeScale = 1f;
        
        for (int i = 0; i < 4; i++)
        {
            IPoker poker = PokerFactory.Product();
            Deck.Add(poker);
        }
        
        Selector.Capacity = 2;
        this.SendEvent(new SelectorEnableChangedEvent
        {
            Enable = true
        });
    }

    public override void _Process(double delta)
    {
        _log.Debug($"{Selector.Count}");
    }
}