using TimeToTwentyfour.global;

namespace TimeToTwentyfour.scripts.entities.poker;

public partial class PokerHolder
{ 
    private async Task ReadyAsync()
    {
        // 等待框架加载完成
        await GameEntryPoint.Architecture.WaitUntilReadyAsync().ConfigureAwait(false);
    }
}