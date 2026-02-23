using GFramework.Core.Abstractions.controller;
using GFramework.Core.Abstractions.state;
using GFramework.Core.extensions;
using GFramework.Game.Abstractions.enums;
using GFramework.Game.Abstractions.ui;
using GFramework.Godot.ui;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using GFrameworkGodotTemplate.global;
using GFrameworkGodotTemplate.scripts.command.menu.select;
using GFrameworkGodotTemplate.scripts.core.state.impls;
using GFrameworkGodotTemplate.scripts.core.ui;
using GFrameworkGodotTemplate.scripts.enums.ui;
using Godot;

namespace GFrameworkGodotTemplate.scripts.select_menu;

[ContextAware]
[Log]
public partial class SelectMenu : Control, IController, IUiPageBehaviorProvider, ISimpleUiPage
{
    private TextureButton PrevButton => GetNode<TextureButton>("%PrevButton");
    private Label PileName => GetNode<Label>("%PileName");
    private TextureButton NextButton => GetNode<TextureButton>("%NextButton");
    private Button CheckButton => GetNode<Button>("%CheckButton");
    private Button ReturnButton => GetNode<Button>("%ReturnButton");
    private TextureButton Pile => GetNode<TextureButton>("%Pile");
    
    private IUiPageBehavior? _page;
    private IStateMachineSystem _stateMachineSystem = null!;
    private IUiRouter _uiRouter = null!;
    public static string UiKeyStr => nameof(UiKey.SelectMenu);
    
    [Export] public ColorRect[] ThemeColorBlock = new ColorRect[]{};
    
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

    public override void _Process(double delta)
    {
        // Pile.GlobalPosition = GetGlobalMousePosition() - Pile.Size / 2;
    }

    private async Task ReadyAsync()
    {
        await GameEntryPoint.Architecture.WaitUntilReadyAsync().ConfigureAwait(false);
        _uiRouter = this.GetSystem<IUiRouter>()!;
        _stateMachineSystem = this.GetSystem<IStateMachineSystem>()!;
    }

    private void ConnectSignal()
    {
        PrevButton.ButtonDown += () => ContextAwareExtensions.SendCommand(this, new SelectPrevPileCommand());
        NextButton.ButtonDown += () => ContextAwareExtensions.SendCommand(this, new SelectNextPileCommand());
        CheckButton.ButtonDown += () => _stateMachineSystem.ChangeTo<ClockMenuState>();
        ReturnButton.ButtonDown += () => _stateMachineSystem.ChangeTo<MainMenuState>();
        Pile.ButtonDown += () => { GD.Print("click"); };
    }
}