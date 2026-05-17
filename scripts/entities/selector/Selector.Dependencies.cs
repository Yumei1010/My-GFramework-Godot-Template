using TimeToTwentyfour.global;

namespace TimeToTwentyfour.scripts.entities.selector;

public partial class Selector
{
    private async Task ReadyAsync()
    {
        // 等待框架加载完成
        await GameEntryPoint.Architecture.WaitUntilReadyAsync().ConfigureAwait(false);
    }
}
