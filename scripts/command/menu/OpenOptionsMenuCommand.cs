using GFramework.Core.command;
using GFramework.Core.extensions;
using GFramework.Game.Abstractions.enums;
using GFramework.Game.Abstractions.ui;
using GFrameworkGodotTemplate.scripts.options_menu;

namespace GFrameworkGodotTemplate.scripts.command.menu;

/// <summary>
///     命令类，用于打开选项菜单。
///     继承自 AbstractCommand，实现具体的执行逻辑。
/// </summary>
public class OpenOptionsMenuCommand : AbstractCommand
{
    /// <summary>
    ///     执行命令的核心方法。
    ///     通过获取 UI 路由系统，显示选项菜单界面。
    /// </summary>
    protected override void OnExecute()
    {
        // 获取 UI 路由系统实例，并调用 Show 方法显示选项菜单
        this.GetSystem<IUiRouter>()!.Show(OptionsMenu.UiKeyStr, UiLayer.Modal);
    }
}