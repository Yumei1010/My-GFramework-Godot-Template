using GFramework.Core.Abstractions.state;
using GFramework.Core.extensions;
using GFramework.Core.state;
using GFramework.Game.Abstractions.scene;
using GFramework.Game.Abstractions.ui;
using GFrameworkGodotTemplate.scripts.select_menu;

namespace GFrameworkGodotTemplate.scripts.core.state.impls;

/// <summary>
///     选择牌组菜单状态
///     负责管理选择牌组菜单界面的显示和隐藏逻辑
/// </summary>
public class SelectMenuState : ContextAwareStateBase
{
    /// <summary>
    ///     状态进入时的处理方法
    /// </summary>
    /// <param name="from">从哪个状态切换过来，可能为空</param>
    public override void OnEnter(IState? from)
    {
        // 回到选择牌组菜单需要销毁其它所有Ui界面以及场景
        var uiRouter = this.GetSystem<IUiRouter>()!;
        uiRouter.Clear();
        this.GetSystem<ISceneRouter>()!.Unload();
        // 推送选择牌组菜单UI到界面栈中，显示选择牌组菜单界面
        uiRouter.Push(SelectMenu.UiKeyStr);
    }

    /// <summary>
    ///     判断是否可以切换到下一个状态
    /// </summary>
    /// <param name="target">目标状态</param>
    /// <returns>始终返回true，表示可以切换到任意状态</returns>
    public override bool CanTransitionTo(IState target)
    {
        return true;
    }
}