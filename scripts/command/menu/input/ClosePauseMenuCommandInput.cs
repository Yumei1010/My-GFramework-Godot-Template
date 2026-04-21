using GFramework.Core.Abstractions.command;
using GFramework.Game.Abstractions.ui;

namespace GFrameworkGodotTemplate.scripts.command.menu.input;

/// <summary>
///     表示关闭暂停菜单命令的输入数据结构。
/// </summary>
public struct ClosePauseMenuCommandInput : ICommandInput
{
    /// <summary>
    ///     用于标识和操作UI元素的句柄。
    /// </summary>
    public UiHandle Handle { get; init; }
}