using GFramework.Core.Extensions;
using GFramework.Godot.Extensions;
using GFrameworkTemplate.scripts.cqrs.game.@event;

namespace GFrameworkTemplate.scripts.ui.menu;

/// <summary>
///     主菜单页面事件订阅：响应 CQRS 事件刷新自身状态。
/// </summary>
public partial class MainMenu
{
    /// <summary>
    ///     注册事件订阅（节点退出场景树时自动注销）。
    /// </summary>
    private void RegisterEvents()
    {
        this.RegisterEvent<GameStartedEvent>(OnGameStarted)
            .UnRegisterWhenNodeExitTree(this);
    }

    /// <summary>
    ///     响应游戏开始事件：把标题切换为运行态文案，演示"命令 → 事件 → UI 响应"闭环。
    /// </summary>
    /// <param name="event">游戏开始事件。</param>
    private void OnGameStarted(GameStartedEvent @event)
    {
        var title = _localization.GetText("common", "app.title");
        var running = _localization.GetText("common", "ui.state.running");
        _titleLabel.Text = $"{title} · {running}";
        _log.Info("收到 GameStartedEvent，主菜单已刷新标题");
    }
}