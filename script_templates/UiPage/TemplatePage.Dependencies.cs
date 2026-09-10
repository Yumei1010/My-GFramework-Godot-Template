using System;
using System.Threading.Tasks;
using GFramework.Core.SourceGenerators.Abstractions.Rule;
using GFramework.Game.Abstractions.UI;
using GFramework.Godot.SourceGenerators.Abstractions;
using Godot;
using GFrameworkTemplate.global;

namespace GFrameworkTemplate.scripts.ui.menu;

public partial class TemplatePage
{
    /// <summary>
    ///     UI 路由器（架构依赖注入）
    /// </summary>
    [GetSystem]
    private IUiRouter _uiRouter = null!;

    /// <summary>
    ///     页面节点引用：[GetNode] 按字段名推断 %唯一名（_titleLabel → %TitleLabel）
    ///     供 [BindNodeSignal] 信号绑定使用
    /// </summary>
    [GetNode]
    private Label _titleLabel = null!;

    [GetNode]
    private Button _startButton = null!;

    /// <summary>
    ///     异步等待架构就绪（若页面逻辑依赖就绪后的系统/模型可在此继续）
    /// </summary>
    private async Task ReadyAsync()
    {
        try
        {
            // 不要使用 ConfigureAwait(false)：续体会被切到线程池，之后访问 Godot 节点 API 会失败
            await GameEntryPoint.Architecture.WaitUntilReadyAsync();
            _log.Debug("TemplatePage 初始化完成");
        }
        catch (Exception ex)
        {
            // _ = ReadyAsync() 不观察异常，显式记录避免初始化失败静默
            _log.Error("TemplatePage 初始化失败", ex);
        }
    }
}
