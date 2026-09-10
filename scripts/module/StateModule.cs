using GFramework.Core.Abstractions.Architectures;
using GFramework.Core.Functional.Pipe;
using GFramework.Game.State;
using GFrameworkTemplate.scripts.core.state;

namespace GFrameworkTemplate.scripts.module;

/// <summary>
///     状态模块类，负责安装和注册应用状态机及状态
/// </summary>
public class StateModule : IArchitectureModule
{
    /// <summary>
    ///     安装 StateModule：注册状态机系统与状态。
    /// </summary>
    /// <param name="architecture">目标架构。</param>
    public void Install(IArchitecture architecture)
    {
        architecture.RegisterSystem(new GameStateMachineSystem().Also(it =>
        {
            it.Register(new AppState());
        }));
    }
}
