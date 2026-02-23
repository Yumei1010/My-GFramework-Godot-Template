using GFramework.Core.Abstractions.state;
using GFramework.Core.extensions;
using GFramework.Core.state;
using GFramework.Game.Abstractions.ui;
using GFrameworkGodotTemplate.scripts.select_menu;
using SelectMenu = GFrameworkGodotTemplate.scripts.select_menu.SelectMenu;

namespace GFrameworkGodotTemplate.scripts.core.state.impls;

/// <summary>
///     游戏进行中状态
///     表示游戏当前处于运行阶段的状态管理类。
///     继承自ContextAwareStateBase，用于处理游戏运行时的逻辑。
/// </summary>
public class PlayingState : ContextAwareStateBase
{
    public override void OnEnter(IState? from)
    {
        // this.GetSystem<IUiRouter>()!.Replace(SelectMenu.UiKeyStr);
    }
}