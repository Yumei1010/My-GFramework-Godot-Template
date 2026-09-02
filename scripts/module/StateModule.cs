using GFramework.Core.Abstractions.Architectures;
using GFramework.Core.Functional.Pipe;
using GFramework.Game.State;
using GFrameworkTemplate.scripts.core.state.impls;

namespace GFrameworkTemplate.scripts.module;

/// <summary>
///     状态模块类，负责安装和注册应用状态机及状态
/// </summary>
public class StateModule : IArchitectureModule
{
    public void Install(IArchitecture architecture)
    {
        architecture.RegisterSystem(new GameStateMachineSystem().Also(it =>
        {
            it.Register(new AppState());
        }));
    }
}
