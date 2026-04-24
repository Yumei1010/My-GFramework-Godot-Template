using TimeToTwentyfour.global;

namespace TimeToTwentyfour.scripts.menu.calculate_menu;

public partial class CalculateMenu
{
    private ICalculateMenuOptionButton PlayButton => GetNode<ICalculateMenuOptionButton>("PlayButton");
    private ICalculateMenuOptionButton ModeButton => GetNode<ICalculateMenuOptionButton>("ModeButton");
    
    private async Task ReadyAsync()
    {
        // 等待框架加载完成
        await GameEntryPoint.Architecture.WaitUntilReadyAsync().ConfigureAwait(false);
    }
}