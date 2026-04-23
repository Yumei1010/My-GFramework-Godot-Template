using GFramework.Core.Abstractions.controller;
using GFramework.Core.Abstractions.state;
using GFramework.Core.extensions;
using GFramework.Game.Abstractions.enums;
using GFramework.Game.Abstractions.ui;
using GFramework.Godot.ui;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using GFrameworkGodotTemplate.scripts.core.state.impls;
using GFrameworkGodotTemplate.scripts.core.ui;
using GFrameworkGodotTemplate.scripts.cqrs.game.command;
using GFrameworkGodotTemplate.scripts.enums.ui;
using global::GFrameworkGodotTemplate.global;
using Godot;

namespace GFrameworkGodotTemplate.scripts.menu.main_menu;

[ContextAware]
[Log]
public partial class MainMenu : Control, IController, IUiPageBehaviorProvider, ISimpleUiPage
{
    private Button NewGameButton => GetNode<Button>("%NewGameButton");
    private Button OptionsMenuButton => GetNode<Button>("%OptionsMenuButton");
    private Button CreditsButton => GetNode<Button>("%CreditsButton");
    private Button ExitButton => GetNode<Button>("%ExitButton");
    
    private IUiPageBehavior? _page;
    private IStateMachineSystem _stateMachineSystem = null!;
    public static string UiKeyStr => nameof(UiKey.MainMenu);
    
    public IUiPageBehavior GetPage()
    {
        _page ??= UiPageBehaviorFactory.Create<Control>(this, UiKeyStr, UiLayer.Page);
        return _page;
    }
    
    public override void _Ready()
    {
        _ = ReadyAsync();
        ConnectSignal();
    }

    private async Task ReadyAsync()
    {
        await GameEntryPoint.Architecture.WaitUntilReadyAsync().ConfigureAwait(false);
        _stateMachineSystem = ContextAwareExtensions.GetSystem<IStateMachineSystem>(this)!;
    }
    
    private void ConnectSignal()
    {
        NewGameButton.ButtonDown += OnMouseDownNewGameButton;                     
        OptionsMenuButton.ButtonDown += OnMouseDownOptionsMenuButton;
        CreditsButton.ButtonDown += OnMouseDownCreditsButton;
        ExitButton.ButtonDown += OnMouseDownExitButton;
    }

    private void OnMouseDownNewGameButton()
    {
        _stateMachineSystem.ChangeTo<CalculateMenuState>();
    }

    private void OnMouseDownOptionsMenuButton()
    {
        _stateMachineSystem.ChangeTo<OptionsMenuState>();
    }

    private void OnMouseDownCreditsButton()
    {
        _stateMachineSystem.ChangeTo<CreditsState>();
    }

    private void OnMouseDownExitButton()
    {
        ContextAwareExtensions.SendCommand(this, new ExitGameCommand());
    }
}