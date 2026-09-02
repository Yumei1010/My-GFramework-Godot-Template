// meta-name: 简单UI页面控制器类模板
// meta-description: 负责管理UI页面场景的生命周期和架构关联
using Godot;
using GFramework.Core.Abstractions.Controller;
using GFramework.Core.Extensions;
using GFramework.Game.Abstractions.Enums;
using GFramework.Game.Abstractions.UI;
using GFramework.Godot.UI;
using GFramework.Core.SourceGenerators.Abstractions.Logging;
using GFramework.Core.SourceGenerators.Abstractions.Rule;
using GFrameworkTemplate.scripts.core.ui;
using GFrameworkTemplate.scripts.enums.ui;
using GFrameworkTemplate.global;

/// <summary>
///     _CLASS_ UI 页面——继承 ISimpleUiPage 的标准页面模式
///     建议按 partial 五文件模式拆分：.cs / .Dependencies.cs / .Properties.cs / .Events.cs / .Signals.cs
/// </summary>
[Log]
[ContextAware]
public partial class _CLASS_ : _BASE_, IController, IUiPageBehaviorProvider, ISimpleUiPage
{
    private IUiPageBehavior? _page;
    private IUiRouter _uiRouter = null!;
    /// <summary>
    ///     UI 键的字符串形式（需先在 UiKey 枚举中注册）
    /// </summary>
    public static string UiKeyStr => nameof(UiKey._CLASS_);
    /// <summary>
    ///     Godot 节点就绪回调，按顺序执行：异步初始化 → 信号绑定 → 事件注册
    /// </summary>
    public override void _Ready()
    {
        _ = ReadyAsync();
        ConnectPageSignals();
        RegisterEvents();
    }
    /// <summary>
    ///     获取当前页面的 UI 行为实例
    /// </summary>
    public IUiPageBehavior GetPage()
    {
        _page ??= UiPageBehaviorFactory.Create<_BASE_>(this, UiKeyStr, UiLayer.Page);
        return _page;
    }
    /// <summary>
    ///     异步等待架构就绪，获取 UI 路由器依赖
    /// </summary>
    private async Task ReadyAsync()
    {
        await GameEntryPoint.Architecture.WaitUntilReadyAsync().ConfigureAwait(false);
        _uiRouter = this.GetSystem<IUiRouter>()!;
        _log.Debug("_CLASS_ 初始化完成");
    }
    /// <summary>
    ///     连接 Godot 信号并桥接到 CQRS 事件（模板用户在此处添加信号绑定逻辑）
    /// </summary>
    private void ConnectPageSignals()
    {
        // 示例：Button.Pressed += () => this.SendEvent(new ButtonPressedEvent { ... });
    }
    /// <summary>
    ///     注册 CQRS 事件订阅（模板用户在此处添加事件处理逻辑）
    /// </summary>
    private void RegisterEvents()
    {
        // 示例：this.RegisterEvent<SomeEvent>(e => { ... })
        //     .UnRegisterWhenNodeExitTree(this);
    }
}