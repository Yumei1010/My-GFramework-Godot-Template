using GFramework.Core.Abstractions.controller;
using GFramework.Core.extensions;
using GFramework.Game.Abstractions.enums;
using GFramework.Game.Abstractions.ui;
using GFramework.Godot.ui;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using GFrameworkGodotTemplate.scripts.core.ui;
using GFrameworkGodotTemplate.scripts.enums.ui;
using GFrameworkGodotTemplate.global;
using GFrameworkGodotTemplate.scripts.events.pokerSelector;
using Godot;

namespace GFrameworkGodotTemplate.scripts.calculate_menu;

[ContextAware]
[Log]
public partial class CalculateMenu : Control, IController, IUiPageBehaviorProvider, ISimpleUiPage
{
    private Button Button => GetNode<Button>("%Button");
    private Button Button2 => GetNode<Button>("%Button2");
    
    private IUiPageBehavior? _page;
    public static string UiKeyStr => nameof(UiKey.CalculateMenu);
    
    public IUiPageBehavior GetPage()
    {
        _page ??= UiPageBehaviorFactory.Create<Control>(this, UiKeyStr, UiLayer.Page);
        return _page;
    }
    
    public override void _Ready()
    {
        _ = ReadyAsync();
        ConnectSignal();
        RegisterEvent();
    }

    private async Task ReadyAsync()
    {
        await GameEntryPoint.Architecture.WaitUntilReadyAsync().ConfigureAwait(false);
    }

    private void ConnectSignal()
    {
        Button.ButtonDown +=  OnButtonPressed;
        Button2.ButtonDown +=  OnButton2Pressed;
    }

    private void RegisterEvent()
    {
       
    }
    
    private void OnButtonPressed()
    {
        this.SendEvent(new PokerSelectorEnableChangedEvent()
        {
            Enable = true
        });
    }
    
    private void OnButton2Pressed()
    {
        this.SendEvent(new PokerSelectorEnableChangedEvent()
        {
            Enable = false
        });
    }
}