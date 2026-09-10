using System.Threading.Tasks;
using GFramework.Core.Abstractions.Localization;
using GFramework.Core.Extensions;
using GFramework.Core.SourceGenerators.Abstractions.Rule;
using GFramework.Godot.SourceGenerators.Abstractions;
using Godot;
using GFrameworkTemplate.global;

namespace GFrameworkTemplate.scripts.ui.menu;

/// <summary>
///     主菜单页面依赖：架构组件注入（[GetSystem]）、节点引用（[GetNode]）与异步初始化。
/// </summary>
public partial class MainMenu
{
    /// <summary>
    ///     本地化管理器（架构注入）：标题文案按当前语言取词。
    /// </summary>
    [GetSystem]
    private ILocalizationManager _localization = null!;

    /// <summary>
    ///     标题文本节点（唯一名 %TitleLabel）。
    /// </summary>
    [GetNode]
    private Label _titleLabel = null!;

    /// <summary>
    ///     开始按钮（唯一名 %StartButton）。
    /// </summary>
    [GetNode]
    private Button _startButton = null!;

    /// <summary>
    ///     退出按钮（唯一名 %QuitButton）。
    /// </summary>
    [GetNode]
    private Button _quitButton = null!;

    /// <summary>
    ///     等待架构就绪后初始化页面内容。
    /// </summary>
    private async Task ReadyAsync()
    {
        try
        {
            // 关键：不要使用 ConfigureAwait(false)，否则续体会切到线程池，
            // 之后访问 Godot 节点 API 会失败。保留 Godot 同步上下文即可。
            await GameEntryPoint.Architecture.WaitUntilReadyAsync();
            RefreshTitle();
            _log.Debug("MainMenu 初始化完成");
        }
        catch (Exception ex)
        {
            // _ = ReadyAsync() 不观察异常，这里显式记录避免初始化失败静默
            _log.Error("MainMenu 初始化失败", ex);
        }
    }

    /// <summary>
    ///     按当前语言刷新标题文本。
    /// </summary>
    private void RefreshTitle()
    {
        _titleLabel.Text = _localization.GetText("common", "app.title");
    }
}