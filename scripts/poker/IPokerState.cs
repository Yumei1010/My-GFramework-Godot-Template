using GFrameworkGodotTemplate.scripts.stateMachine;

namespace GFrameworkGodotTemplate.scripts.poker;

/// <summary>
/// 扑克状态接口，定义了扑克状态的基本属性和必须实现的功能
/// </summary>
public interface IPokerState : IState
{
    /// <summary>
    ///     设置扑克状态的当前代理扑克。
    /// </summary>
    /// <param name="poker">要设置的扑克 <see cref="IPoker"/> 实例</param>
    void SetPoker(IPoker poker);
    
    /// <summary>
    ///     鼠标点击时调用的方法
    /// </summary>
    void MouseDown();

    /// <summary>
    ///     鼠标释放时调用的方法
    /// </summary>
    void MouseUp();

    /// <summary>
    ///     鼠标进入时调用的方法
    /// </summary>
    void MouseEnter();

    /// <summary>
    ///     鼠标离开时调用的方法
    /// </summary>
    void MouseExit();
}