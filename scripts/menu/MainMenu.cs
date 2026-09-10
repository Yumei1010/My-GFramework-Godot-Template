using GFramework.Core.Abstractions.Controller;
using GFramework.Core.SourceGenerators.Abstractions.Logging;
using GFramework.Core.SourceGenerators.Abstractions.Rule;
using GFramework.Game.Abstractions.Enums;
using GFramework.Game.Abstractions.UI;
using GFramework.Godot.SourceGenerators.Abstractions;
using GFramework.Godot.SourceGenerators.Abstractions.UI;
using Godot;
using GFrameworkTemplate.scripts.core.ui;
using GFrameworkTemplate.scripts.enums.ui;

namespace GFrameworkTemplate.scripts.menu;

/// <summary>
///     主菜单页面——模板真实页面示例（语法糖 + CQRS 端到端链路）。
/// </summary>
/// <remarks>
///     演示要点：
///     <list type="bullet">
///         <item><see cref="AutoUiPageAttribute" /> 自动生成 <c>UiKeyStr</c> / <c>GetPage()</c> 页面样板</item>
///         <item><c>[GetSystem]</c> 字段注入架构组件、<c>[GetNode]</c> 节点注入、<c>[BindNodeSignal]</c> 信号绑定</item>
///         <item>按钮 → 命令 → 事件 → 页面响应 的完整链路（见 .Signals.cs / .Events.cs）</item>
///         <item>标题文案取自本地化表（见 .Dependencies.cs）</item>
///     </list>
/// </remarks>
[Log]
[ContextAware]
[AutoUiPage(nameof(UiKey.MainMenu), nameof(UiLayer.Page))]
public partial class MainMenu : Control, IController, IUiPageBehaviorProvider, ISimpleUiPage
{
    /// <summary>
    ///     节点就绪：节点注入 → 信号绑定 → 异步初始化 → 事件注册。
    /// </summary>
    public override void _Ready()
    {
        __InjectContextBindings_Generated();   // [GetSystem] 等架构组件字段注入
        __InjectGetNodes_Generated();          // [GetNode] 节点字段注入
        __BindNodeSignals_Generated();         // [BindNodeSignal] 信号绑定
        RegisterEvents();                      // 先订阅事件，避免错过初始化期间发出的事件
        _ = ReadyAsync();                      // 再做异步初始化（架构就绪等待）
    }

    /// <summary>
    ///     退出场景树：解绑生成器绑定的信号（防泄漏）。
    /// </summary>
    public override void _ExitTree()
    {
        __UnbindNodeSignals_Generated();
    }
}