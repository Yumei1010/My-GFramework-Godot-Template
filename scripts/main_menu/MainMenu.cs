using GFramework.Core.Abstractions.controller;
using GFramework.Core.Abstractions.state;
using GFramework.Core.extensions;
using GFramework.Game.Abstractions.enums;
using GFramework.Game.Abstractions.ui;
using GFramework.Godot.ui;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using GFrameworkGodotTemplate.scripts.command.game;
using GFrameworkGodotTemplate.scripts.command.menu;
using GFrameworkGodotTemplate.scripts.core.state.impls;
using GFrameworkGodotTemplate.scripts.core.ui;
using GFrameworkGodotTemplate.scripts.credits;
using GFrameworkGodotTemplate.scripts.enums.ui;
using GFrameworkGodotTemplate.global;
using Godot;

namespace GFrameworkGodotTemplate.scripts.main_menu;

[ContextAware]
[Log]
public partial class MainMenu : Control, IController, IUiPageBehaviorProvider, ISimpleUiPage
{
    [Export] public ColorRect[] ThemeColorBlock = [];
    
    private Button NewGameButton => GetNode<Button>("%NewGameButton");
    private Button ContinueGameButton => GetNode<Button>("%ContinueGameButton");
    private Button OptionsMenuButton => GetNode<Button>("%OptionsMenuButton");
    private Button CreditsButton => GetNode<Button>("%CreditsButton");
    private Button ExitButton => GetNode<Button>("%ExitButton");
    
    private IUiPageBehavior? _page;
    private IStateMachineSystem _stateMachineSystem = null!;
    private IUiRouter _uiRouter = null!;
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
        _uiRouter = this.GetSystem<IUiRouter>()!;
        _stateMachineSystem = this.GetSystem<IStateMachineSystem>()!;
    }
    
    private void ConnectSignal()
    {
        NewGameButton.ButtonDown += () => _stateMachineSystem.ChangeTo<SelectMenuState>();
        ContinueGameButton.ButtonDown += () => { };
        OptionsMenuButton.ButtonDown += () => this.SendCommand(new OpenOptionsMenuCommand());
        CreditsButton.ButtonDown += () => _uiRouter.Push(Credits.UiKeyStr);
        ExitButton.ButtonDown += () => this.SendCommand(new ExitGameCommand());
    }
}