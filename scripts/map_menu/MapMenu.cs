using GFramework.Core.Abstractions.controller;
using GFramework.Core.Abstractions.state;
using GFramework.Core.extensions;
using GFramework.Game.Abstractions.enums;
using GFramework.Game.Abstractions.ui;
using GFramework.Godot.ui;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using GFrameworkGodotTemplate.global;
using GFrameworkGodotTemplate.scripts.core.ui;
using GFrameworkGodotTemplate.scripts.enums.ui;
using Godot;

namespace GFrameworkGodotTemplate.scripts.map_menu;

[ContextAware]
[Log]
public partial class MapMenu : Control, IController, IUiPageBehaviorProvider, ISimpleUiPage
{
    private IUiPageBehavior? _page;
    public static string UiKeyStr => nameof(UiKey.MapMenu);
    
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
    }
}
