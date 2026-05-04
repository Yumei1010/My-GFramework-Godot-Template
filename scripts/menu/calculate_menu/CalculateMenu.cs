using GFramework.Core.Abstractions.controller;
using GFramework.Game.Abstractions.enums;
using GFramework.Game.Abstractions.ui;
using GFramework.Godot.ui;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using TimeToTwentyfour.scripts.core.ui;
using Godot;

namespace TimeToTwentyfour.scripts.menu.calculate_menu;

/// <summary>计算菜单页面：提供运算符选择、选牌确认、弃牌、排序及时间条控制。</summary>
[Log]
[ContextAware]
public partial class CalculateMenu : Control, IController, IUiPageBehaviorProvider, ISimpleUiPage
{
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
}