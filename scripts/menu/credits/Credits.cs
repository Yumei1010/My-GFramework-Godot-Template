using GFramework.Core.Abstractions.controller;
using GFramework.Core.Abstractions.state;
using GFramework.Core.extensions;
using GFramework.Game.Abstractions.enums;
using GFramework.Game.Abstractions.ui;
using GFramework.Godot.ui;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using TimeToTwentyfour.scripts.constants;
using TimeToTwentyfour.scripts.core.ui;
using TimeToTwentyfour.scripts.enums.ui;
using TimeToTwentyfour.global;
using Godot;
using TimeToTwentyfour.scripts.core.state.impls;
using TimeToTwentyfour.scripts.menu.main_menu;

namespace TimeToTwentyfour.scripts.menu.credits;

[Log]
[ContextAware]
public partial class Credits : Control, IController, IUiPageBehaviorProvider, ISimpleUiPage
{
    private Button BackButton => GetNode<Button>("%BackButton");

    private IStateMachineSystem _stateMachineSystem = null!;
    private IUiRouter _uiRouter = null!;
    
    private IUiPageBehavior? _page;
    public static string UiKeyStr => nameof(UiKey.Credits);

    /// <summary>
    ///     获取页面行为实例，如果不存在则创建新的CanvasItemUiPageBehavior实例
    /// </summary>
    /// <returns>返回IUiPageBehavior类型的页面行为实例</returns>
    public IUiPageBehavior GetPage()
    {
        _page ??= UiPageBehaviorFactory.Create<Control>(this, UiKeyStr, UiLayer.Page);
        return _page;
    }

    /// <summary>
    ///     检查当前UI是否在路由栈顶，如果不在则将页面推入路由栈
    /// </summary>
    private void CallDeferredInit()
    {
        var env = ContextAwareExtensions.GetEnvironment(this);
        // 开发环境下检查当前UI是否在路由栈顶，如果不在则将页面推入路由栈
        if (GameConstants.Development.Equals(env.Name, StringComparison.Ordinal) && !_uiRouter.IsTop(UiKeyStr))
            _uiRouter.Push(GetPage());
        // 在此添加延迟初始化逻辑
    }

    /// <summary>
    ///     节点准备就绪时的回调方法
    ///     在节点添加到场景树后调用
    /// </summary>
    public override void _Ready()
    {
        _ = ReadyAsync();
        ConnectSignal();
    }

    /// <summary>
    ///     异步等待架构准备完成并获取UI路由器系统
    /// </summary>
    private async Task ReadyAsync()
    {
        // 等待框架加载完成
        await GameEntryPoint.Architecture.WaitUntilReadyAsync().ConfigureAwait(false);
        
        // 依赖注入
        _stateMachineSystem = this.GetSystem<IStateMachineSystem>()!;
        _uiRouter = this.GetSystem<IUiRouter>()!;
    }

    private void ConnectSignal()
    {
        BackButton.Pressed += OnBackButton;
    }

    private void OnBackButton()
    {
        _stateMachineSystem.ChangeTo<MainMenuState>();
    }
}