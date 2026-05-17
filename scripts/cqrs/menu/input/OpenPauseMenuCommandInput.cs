using GFramework.Core.Abstractions.command;
using GFramework.Game.Abstractions.ui;

namespace TimeToTwentyfour.scripts.cqrs.menu.input;

/// <summary>
///     表示打开暂停菜单命令的输入类。
/// </summary>
public sealed class OpenPauseMenuCommandInput : ICommandInput
{
    /// <summary>
    ///     用于标识和操作UI元素的句柄。
    /// </summary>
    public UiHandle? Handle { get; set; }
}