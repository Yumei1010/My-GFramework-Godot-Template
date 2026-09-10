using GFramework.Core.Extensions;
using GFramework.Godot.SourceGenerators.Abstractions;
using Godot;
using GFrameworkTemplate.scripts.cqrs.game.command;

namespace GFrameworkTemplate.scripts.ui.menu;

/// <summary>
///     主菜单页面信号桥接：[BindNodeSignal] 把节点信号绑定为 CQRS 命令发送。
/// </summary>
/// <remarks>
///     Godot 主线程禁止同步 CQRS 调用（框架 GuardSyncCqrs），命令统一以 <c>SendCommandAsync</c> 异步派发。
/// </remarks>
public partial class MainMenu
{
    /// <summary>
    ///     开始按钮按下：发送开始游戏命令（命令内部广播 <c>GameStartedEvent</c>）。
    /// </summary>
    [BindNodeSignal(nameof(_startButton), nameof(Button.Pressed))]
    private void OnStartPressed()
    {
        _ = this.SendCommandAsync(new StartGameCommand());
        _log.Debug("已发送 StartGameCommand");
    }

    /// <summary>
    ///     退出按钮按下：发送退出游戏命令。
    /// </summary>
    [BindNodeSignal(nameof(_quitButton), nameof(Button.Pressed))]
    private void OnQuitPressed()
    {
        _ = this.SendCommandAsync(new ExitGameCommand());
    }
}
