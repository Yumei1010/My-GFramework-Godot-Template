using GFramework.Core.Abstractions.controller;
using GFramework.Game.Abstractions.enums;
using GFramework.Game.Abstractions.ui;
using GFramework.Godot.ui;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using GFrameworkGodotTemplate.global;
using GFrameworkGodotTemplate.scripts.core.ui;
using GFrameworkGodotTemplate.scripts.enums.ui;
using Godot;

namespace GFrameworkGodotTemplate.scripts.archive_menu;

[ContextAware]
[Log]
public partial class ArchiveMenu : Control, IController, IUiPageBehaviorProvider, ISimpleUiPage
{
    private IUiPageBehavior? _page;
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
    }
    
    private void ConnectSignal()
    {

    }
}