using GFramework.Core.command;
using GFramework.Core.extensions;
using GFramework.Game.Abstractions.enums;
using GFramework.Game.Abstractions.ui;
using TimeToTwentyfour.scripts.cqrs.menu.input;

namespace TimeToTwentyfour.scripts.cqrs.menu;

/// <summary>
///     关闭暂停菜单的命令类。
///     该类继承自 AbstractCommand，用于执行关闭暂停菜单的操作。
/// </summary>
public class CloseOptionsMenuCommand(CloseOptionsMenuCommandInput input) : AbstractCommand<CloseOptionsMenuCommandInput>(input)
{
    /// <summary>
    ///     执行关闭暂停菜单的具体逻辑。
    ///     通过获取 UI 路由系统，隐藏指定的暂停菜单界面。
    /// </summary>
    protected override void OnExecute(CloseOptionsMenuCommandInput input)
    {
        // 获取 UI 路由系统实例，并调用 Hide 方法隐藏暂停菜单界面
        this.GetSystem<IUiRouter>()!.Hide(input.Handle, UiLayer.Modal);
    }
}