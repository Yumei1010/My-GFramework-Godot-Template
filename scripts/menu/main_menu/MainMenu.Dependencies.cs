using GFramework.Core.Abstractions.state;
using GFramework.Core.extensions;
using TimeToTwentyfour.global;
using TimeToTwentyfour.scripts.entities.poker;

namespace TimeToTwentyfour.scripts.menu.main_menu;

public partial class MainMenu
{
    private IMainMenuOptionButton StartButton => GetNode<IMainMenuOptionButton>("%StartButton");
    private IMainMenuOptionButton ContinueButton => GetNode<IMainMenuOptionButton>("%ContinueButton");
    private IMainMenuOptionButton SettingButton => GetNode<IMainMenuOptionButton>("%SettingButton");
    private IMainMenuOptionButton CreditsButton => GetNode<IMainMenuOptionButton>("%CreditsButton");
    private IMainMenuOptionButton ExitButton => GetNode<IMainMenuOptionButton>("%ExitButton");
    private IPoker PokerA => GetNode<IPoker>("%PokerA");
    private IPoker PokerB => GetNode<IPoker>("%PokerB");
    private IPoker PokerC => GetNode<IPoker>("%PokerC");
    private IPoker PokerD => GetNode<IPoker>("%PokerD");
    
    private IStateMachineSystem _stateMachineSystem = null!;
    
    private async Task ReadyAsync()
    {
        // 等待框架加载完成
        await GameEntryPoint.Architecture.WaitUntilReadyAsync().ConfigureAwait(false);
        
        // 依赖注入
        _stateMachineSystem = this.GetSystem<IStateMachineSystem>()!;
    }
}