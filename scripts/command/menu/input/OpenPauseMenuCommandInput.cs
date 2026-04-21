using GFramework.Core.Abstractions.command;
using GFramework.Game.Abstractions.ui;

namespace GFrameworkGodotTemplate.scripts.command.menu.input;

public struct OpenPauseMenuCommandInput : ICommandInput
{
    /// <summary>
    ///     用于标识和操作UI元素的句柄。
    /// </summary>
    public UiHandle? Handle { get; init; }
}