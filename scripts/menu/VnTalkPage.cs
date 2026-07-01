using GFramework.Core.Abstractions.controller;
using GFramework.Game.Abstractions.enums;
using GFramework.Game.Abstractions.ui;
using GFramework.Godot.ui;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using GFrameworkTemplate.scripts.core.ui;
using Godot;

namespace GFrameworkTemplate.scripts.menu;

/// <summary>
///     AVG 对话页面——演示视觉小说 UI 的 partial class 模式
///     负责对话文本显示、说话人名称、头像、打字机效果和点击推进
/// </summary>
[Log]
[ContextAware]
public partial class VnTalkPage : Control, IController, IUiPageBehaviorProvider, ISimpleUiPage
{
    private IUiPageBehavior? _page;

    public override void _Ready()
    {
        _ = ReadyAsync();
        ConnectPageSignals();
        RegisterEvents();
    }

    public IUiPageBehavior GetPage()
    {
        _page ??= UiPageBehaviorFactory.Create<VnTalkPage>(this, UiKeyStr, UiLayer.Page);
        return _page;
    }
}
