using GFramework.Core.Abstractions.state;
using GFramework.Core.extensions;
using GFramework.Core.state;
using GFramework.Game.Abstractions.scene;
using GFrameworkGodotTemplate.scripts.enums.scene;

namespace GFrameworkGodotTemplate.scripts.core.state.impls;

/// <summary>
///     表示启动状态的类，继承自 ContextAwareStateBase。
///     该类用于定义系统或应用程序启动时的状态行为。
/// </summary>
public class BootStartState : ContextAwareStateBase
{
    public override void OnEnter(IState? from)
    {
        this.GetSystem<ISceneRouter>()!.Replace(nameof(SceneKey.Boot));
    }

    public override void OnExit(IState? to)
    {
        this.GetSystem<ISceneRouter>()!.Unload();
    }
}