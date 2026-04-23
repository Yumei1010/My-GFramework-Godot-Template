using GFramework.Core.Abstractions.controller;
using GFramework.Core.Abstractions.coroutine;
using GFramework.Core.Abstractions.events;
using GFramework.Core.Abstractions.state;
using GFramework.Core.coroutine.extensions;
using GFramework.Core.coroutine.instructions;
using GFramework.Core.extensions;
using GFramework.Game.Abstractions.enums;
using GFramework.Game.Abstractions.setting;
using GFramework.Game.Abstractions.ui;
using GFramework.Game.setting.events;
using GFramework.Godot.coroutine;
using GFramework.Godot.extensions.signal;
using GFramework.Godot.ui;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using GFrameworkGodotTemplate.scripts.component;
using GFrameworkGodotTemplate.scripts.core.state.impls;
using GFrameworkGodotTemplate.scripts.core.ui;
using GFrameworkGodotTemplate.scripts.enums.ui;
using GFrameworkGodotTemplate.scripts.setting.query;
using GFrameworkGodotTemplate.global;
using GFrameworkGodotTemplate.scripts.cqrs.audio.command;
using GFrameworkGodotTemplate.scripts.cqrs.audio.command.input;
using GFrameworkGodotTemplate.scripts.cqrs.graphics;
using GFrameworkGodotTemplate.scripts.cqrs.graphics.input;
using GFrameworkGodotTemplate.scripts.cqrs.setting;
using Godot;

namespace GFrameworkGodotTemplate.scripts.menu.options_menu;

/// <summary>
///     选项设置界面控制器
///     负责处理游戏设置界面的UI逻辑，包括音量控制、分辨率和全屏模式设置
/// </summary>
[ContextAware]
[Log]
public partial class OptionsMenu : Control, IController, IUiPageBehaviorProvider, ISimpleUiPage
{
    private VolumeContainer MasterVolume => GetNode<VolumeContainer>("%MasterVolumeContainer");
    private VolumeContainer BgmVolume => GetNode<VolumeContainer>("%BgmVolumeContainer");
    private VolumeContainer SfxVolume => GetNode<VolumeContainer>("%SfxVolumeContainer");
    private OptionButton ResolutionOptionButton => GetNode<OptionButton>("%ResolutionOptionButton");
    private OptionButton FullscreenOptionButton => GetNode<OptionButton>("%FullscreenOptionButton");
    private Button BackButton => GetNode<Button>("%BackButton");
    
    private IUiPageBehavior? _page;
    private IStateMachineSystem _stateMachineSystem = null!;
    private IUiRouter _uiRouter = null!;
    public static string UiKeyStr => nameof(UiKey.OptionsMenu);
    
    // 语言选项
    private readonly string[] _languages =
    [
        "简体中文",
        "English"
    ];

    // 分辨率选项
    private readonly Vector2I[] _resolutions =
    [
        new(1920, 1080),
        new(1366, 768),
        new(1280, 720),
        new(1024, 768)
    ];

    private bool _initializing;

    /// <summary>
    ///     获取页面行为实例，如果不存在则创建新的CanvasItemUiPageBehavior实例
    /// </summary>
    /// <returns>返回IUiPageBehavior类型的页面行为实例</returns>
    public IUiPageBehavior GetPage()
    {
        _page ??= UiPageBehaviorFactory.Create<Control>(this, UiKeyStr, UiLayer.Modal);
        return _page;
    }

    /// <summary>
    ///     节点准备就绪时的回调方法
    ///     在节点添加到场景树后调用
    /// </summary>
    public override void _Ready()
    {
        _ = ReadyAsync();
        BackButton.ButtonDown += OnBackPressed;
    }
    
    /// <summary>
    ///     处理未处理的输入事件，用于 ESC 关闭设置窗口
    /// </summary>
    public override void _Input(InputEvent @event)
    {
        if (!@event.IsActionPressed("ui_cancel")) return;
        OnBackPressed();
        AcceptEvent();
    }

    private async Task ReadyAsync()
    {
        // 等待游戏架构初始化完成
        await GameEntryPoint.Architecture.WaitUntilReadyAsync().ConfigureAwait(false);
        SetupEventHandlers();
        _stateMachineSystem = ContextAwareExtensions.GetSystem<IStateMachineSystem>(this)!;
        // 获取UI路由器实例
        _uiRouter = ContextAwareExtensions.GetSystem<IUiRouter>(this)!;
        // 延迟调用初始化方法
        CallDeferred(nameof(CallDeferredInit));
    }
    
    /// <summary>
    ///     检查当前UI是否在路由栈顶，如果不在则将页面推入路由栈
    /// </summary>
    private void CallDeferredInit()
    {
        InitCoroutine().RunCoroutine();
    }

    /// <summary>
    ///     当按下返回键时的处理方法
    /// </summary>
    private void OnBackPressed()
    {
        SaveCommandCoroutine().RunCoroutine(Segment.ProcessIgnorePause);
        _stateMachineSystem.ChangeTo<MainMenuState>();
    }

    /// <summary>
    ///     开始保存设置协程
    /// </summary>
    private void StartSaving()
    {
        SaveCommandCoroutine().RunCoroutine();
    }

    /// <summary>
    ///     初始化用户界面
    ///     设置音量控制组件和分辨率选项的初始值
    /// </summary>
    private void InitializeUi()
    {
        _initializing = true;
        var view = ContextAwareExtensions.SendQuery(this, new GetCurrentSettingsQuery());
        var audioSettings = view.Audio;
        MasterVolume.Initialize("主音量", audioSettings.MasterVolume);
        BgmVolume.Initialize("音乐音量", audioSettings.BgmVolume);
        SfxVolume.Initialize("音效音量", audioSettings.SfxVolume);

        var graphicsSettings = view.Graphics;
        ResolutionOptionButton.Disabled = graphicsSettings.Fullscreen;

        // 初始化全屏选项
        FullscreenOptionButton.Clear();
        FullscreenOptionButton.AddItem("全屏");
        FullscreenOptionButton.AddItem("窗口化");
        FullscreenOptionButton.Selected = graphicsSettings.Fullscreen ? 0 : 1;
        // 初始化分辨率选项
        ResolutionOptionButton.Clear();
        for (var i = 0; i < _resolutions.Length; i++)
        {
            var r = _resolutions[i];
            ResolutionOptionButton.AddItem($"{r.X}x{r.Y}");

            if (r.X == graphicsSettings.ResolutionWidth && r.Y == graphicsSettings.ResolutionHeight)
                ResolutionOptionButton.Selected = i; // ⭐ 正确方式
        }
    }

    /// <summary>
    ///     设置事件处理器
    ///     为音量控制、分辨率和全屏模式选择器绑定事件处理逻辑
    /// </summary>
    private void SetupEventHandlers()
    {
        var signalName = VolumeContainer.SignalName.VolumeChanged;
        MasterVolume
            .Signal(signalName)
            .To(Callable.From<float>(v =>
                CommandCoroutineExtensions.SendCommandCoroutineWithErrorHandler(this, new ChangeMasterVolumeCommand(new ChangeMasterVolumeCommandInput { Volume = v }))
                    .RunCoroutine()))
            .End();
        BgmVolume
            .Signal(signalName)
            .To(Callable.From<float>(v =>
                CommandCoroutineExtensions.SendCommandCoroutineWithErrorHandler(this, new ChangeBgmVolumeCommand(
                            new ChangeBgmVolumeCommandInput { Volume = v }))
                    .RunCoroutine()))
            .End();
        SfxVolume
            .Signal(signalName)
            .To(Callable.From<float>(v =>
                Timing.RunCoroutine(CommandCoroutineExtensions.SendCommandCoroutineWithErrorHandler(this, new ChangeSfxVolumeCommand(
                        new ChangeSfxVolumeCommandInput { Volume = v })))))
            .End();
        ResolutionOptionButton.ItemSelected += async index => await OnResolutionChanged(index).ConfigureAwait(false);
        FullscreenOptionButton.ItemSelected += async index => await OnFullscreenChanged(index).ConfigureAwait(false);
    }

    /// <summary>
    ///     分辨率改变事件
    /// </summary>
    /// <param name="index">选择的分辨率索引</param>
    private async Task OnResolutionChanged(long index)
    {
        if (_initializing) return;
        var resolution = _resolutions[index];
        await ContextAwareExtensions.SendCommandAsync(this, new ChangeResolutionCommand(new ChangeResolutionCommandInput
            { Width = resolution.X, Height = resolution.Y })).ConfigureAwait(false);
        options_menu.OptionsMenu._log.Debug($"分辨率更改为: {resolution.X}x{resolution.Y}");
    }

    /// <summary>
    ///     全屏模式改变事件
    /// </summary>
    /// <param name="index">选择的全屏模式索引</param>
    private async Task OnFullscreenChanged(long index)
    {
        var fullscreen = index == 0;
        await ContextAwareExtensions.SendCommandAsync(this, new ToggleFullscreenCommand(new ToggleFullscreenCommandInput
            { Fullscreen = fullscreen })).ConfigureAwait(false);
        // 禁用 / 启用分辨率选择
        ResolutionOptionButton.Disabled = fullscreen;
        options_menu.OptionsMenu._log.Debug($"全屏模式切换为: {fullscreen}");
    }

    /// <summary>
    ///     初始化协程，用于设置界面的初始化流程
    /// </summary>
    /// <returns>返回一个IYieldInstruction类型的IEnumerator，用于协程执行</returns>
    private IEnumerator<IYieldInstruction> InitCoroutine()
    {
        Hide();
        var settings = ContextAwareExtensions.GetModel<ISettingsModel>(this)!;
        var eventBus = ContextAwareExtensions.GetService<IEventBus>(this)!;
        if (!settings.IsInitialized)
            // 等待设置初始化事件完成
            yield return new WaitForEvent<SettingsInitializedEvent>(eventBus);

        InitializeUi();
        Show();
    }

    /// <summary>
    ///     保存命令协程，用于处理设置保存操作
    /// </summary>
    /// <returns>返回一个IYieldInstruction类型的IEnumerator，用于协程执行</returns>
    private IEnumerator<IYieldInstruction> SaveCommandCoroutine()
    {
        return CommandCoroutineExtensions.SendCommandCoroutineWithErrorHandler(this, new SaveSettingsCommand(),
                e => options_menu.OptionsMenu._log.Error((string)"保存失败！", (Exception)e)
            )
            .Then(() =>
            {
                options_menu.OptionsMenu._log.Info("设置已保存");
                var handle = GetPage().Handle;
                if (handle.HasValue)
                    _uiRouter.Hide(handle.Value, UiLayer.Modal, true);
                else
                    options_menu.OptionsMenu._log.Warn("页面句柄为空，无法隐藏页面");
            });
    }
}