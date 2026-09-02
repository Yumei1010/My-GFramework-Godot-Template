using GFramework.Core.Abstractions.Controller;
using GFramework.Core.SourceGenerators.Abstractions.Logging;
using GFramework.Core.SourceGenerators.Abstractions.Rule;
using GFramework.Game.Abstractions.Enums;
using GFramework.Game.Abstractions.UI;
using GFramework.Godot.SourceGenerators.Abstractions.UI;
using GFrameworkTemplate.scripts.core.ui;
using GFrameworkTemplate.scripts.enums.ui;
using Godot;

namespace GFrameworkTemplate.scripts.menu;

/// <summary>
///     模板页面——语法糖版 UI 页面示例（partial class 模式）
///     <see cref="AutoUiPageAttribute"/> 自动生成 <c>UiKeyStr</c> + <c>GetPage()</c> + 缓存字段样板
/// </summary>
[Log]
[ContextAware]
[AutoUiPage(nameof(UiKey.TemplatePage), nameof(UiLayer.Page))]
public partial class TemplatePage : Control, IController, IUiPageBehaviorProvider, ISimpleUiPage
{
    /// <summary>
    ///     Godot 节点就绪回调，按顺序执行：节点注入 → 异步初始化 → 信号绑定 → 事件注册
    /// </summary>
    public override void _Ready()
    {
        __InjectGetNodes_Generated();  // [GetNode] 字段注入（由生成器提供）
        _ = ReadyAsync();
        ConnectPageSignals();
        RegisterEvents();
    }
}
