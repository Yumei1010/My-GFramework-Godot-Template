using GFramework.Core.Abstractions.state;
using GFramework.Core.extensions;
using TimeToTwentyfour.global;
using Godot;

namespace TimeToTwentyfour.scripts.menu.main_menu;

public partial class MainMenu
{
    private MainMenuOptionButton StartButton => GetNode<MainMenuOptionButton>("%StartButton");
    private MainMenuOptionButton ContinueButton => GetNode<MainMenuOptionButton>("%ContinueButton");
    private MainMenuOptionButton OptionsMenuButton => GetNode<MainMenuOptionButton>("%OptionsMenuButton");
    private MainMenuOptionButton CreditsButton => GetNode<MainMenuOptionButton>("%CreditsButton");
    private MainMenuOptionButton ExitButton => GetNode<MainMenuOptionButton>("%ExitButton");
    
    private IStateMachineSystem _stateMachineSystem = null!;
    
    private async Task ReadyAsync()
    {
        await GameEntryPoint.Architecture.WaitUntilReadyAsync().ConfigureAwait(false);
        _stateMachineSystem = this.GetSystem<IStateMachineSystem>()!;
    }
}