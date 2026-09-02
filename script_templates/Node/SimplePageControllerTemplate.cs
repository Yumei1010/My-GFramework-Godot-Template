// meta-name: 简单UI页面控制器类模板
// meta-description: 负责管理UI页面场景的生命周期和架构关联（语法糖版：AutoUiPage + GetNode）
using Godot;
using GFramework.Core.Abstractions.Controller;
using GFramework.Core.Extensions;
using GFramework.Game.Abstractions.Enums;
using GFramework.Game.Abstractions.UI;
using GFramework.Godot.SourceGenerators.Abstractions;
using GFramework.Godot.SourceGenerators.Abstractions.UI;
using GFramework.Core.SourceGenerators.Abstractions.Logging;
using GFramework.Core.SourceGenerators.Abstractions.Rule;
using GFrameworkTemplate.scripts.core.ui;
using GFrameworkTemplate.scripts.enums.ui;
using GFrameworkTemplate.global;

/// <summary>
///     _CLASS_ UI 页面——语法糖模板
///     [AutoUiPage] 自动生成 UiKeyStr + GetPage() + 缓存字段；
///     节点引用用 [GetNode] 字段注入（_Ready 需调用 __InjectGetNodes_Generated()）
/// </summary>
[Log]
[ContextAware]
[AutoUiPage(nameof(UiKey._CLASS_), nameof(UiLayer.Page))]
public partial class _CLASS_ : _BASE_, IController, IUiPageBehaviorProvider, ISimpleUiPage
{
    // 示例：节点字段注入（字段名 → %唯一名，场景需配置 unique_name_in_owner）
    // [GetNode] private Button _startButton = null!;

    /// <summary>
    ///     Godot 节点就绪回调，按顺序执行：节点注入 → 异步初始化 → 信号绑定 → 事件注册
    /// </summary>
    public override void _Ready()
    {
        // __InjectGetNodes_Generated(); // 启用 [GetNode] 字段后取消注释
        _ = ReadyAsync();
        ConnectPageSignals();
        RegisterEvents();
    }

    /// <summary>
    ///     异步等待架构就绪，获取 UI 路由器依赖
    /// </summary>
    private async Task ReadyAsync()
    {
        await GameEntryPoint.Architecture.WaitUntilReadyAsync().ConfigureAwait(false);
        // _uiRouter = this.GetSystem<IUiRouter>()!;  或 [GetSystem] 字段注入
        _log.Debug("_CLASS_ 初始化完成");
    }

    /// <summary>
    ///     连接 Godot 信号并桥接到 CQRS 事件（模板用户在此处添加信号绑定逻辑）
    /// </summary>
    private void ConnectPageSignals()
    {
        // 示例：_startButton.Pressed += () => this.SendEvent(new ButtonPressedEvent { ... });
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
