using GFramework.Core.Abstractions.controller;
using GFramework.Core.Abstractions.state;
using GFramework.Core.extensions;
using GFramework.Game.Abstractions.enums;
using GFramework.Game.Abstractions.ui;
using GFramework.Godot.ui;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using GFrameworkGodotTemplate.scripts.core.ui;
using GFrameworkGodotTemplate.scripts.enums.ui;
using GFrameworkGodotTemplate.global;
using Godot;

namespace GFrameworkGodotTemplate.scripts.clock_menu;

[ContextAware]
[Log]
public partial class ClockMenu : Control, IController, IUiPageBehaviorProvider, ISimpleUiPage
{
    private IUiPageBehavior? _page;
    private IStateMachineSystem _stateMachineSystem = null!;
    private IUiRouter _uiRouter = null!;
    public static string UiKeyStr => nameof(UiKey.ClockMenu);
    
    public IUiPageBehavior GetPage()
    {
        _page ??= UiPageBehaviorFactory.Create<Control>(this, UiKeyStr, UiLayer.Page);
        return _page;
    }
    
    public override void _Ready()
    {
        _ = ReadyAsync();
    }

    private async Task ReadyAsync()
    {
        await GameEntryPoint.Architecture.WaitUntilReadyAsync().ConfigureAwait(false);
        _uiRouter = this.GetSystem<IUiRouter>()!;
        _stateMachineSystem = this.GetSystem<IStateMachineSystem>()!;
    }
}
