using Godot;
using TimeToTwentyfour.scripts.component.stateMachine;
using TimeToTwentyfour.scripts.entities.poker.stateMachine;
using TimeToTwentyfour.scripts.enums.poker;

namespace TimeToTwentyfour.scripts.entities.poker.state;

/// <summary>
///     扑克状态契约
///     定义一张扑克在特定 <see cref="StateType"/> 下的交互行为与生命周期。
///     继承 <see cref="IState"/>，可被 <see cref="IPokerStateMachine"/> 统一管理。
/// </summary>
/// <remarks>
///     <para>
///         一张扑克在运行时会处于下列状态之一：
///     </para>
///     <list type="table">
///         <listheader><term>状态</term><description>说明</description></listheader>
///         <item><term><see cref="StateType.Idle"/></term><description>闲置</description></item>
///         <item><term><see cref="StateType.Drag"/></term><description>拖动</description></item>
///         <item><term><see cref="StateType.UnSelect"/></term><description>未选中</description></item>
///         <item><term><see cref="StateType.OnSelect"/></term><description>被选中</description></item>
///     </list>
///     <para>
///         状态切换通过 <see cref="ChangeTo"/> 发送
///         <c>PokerStateChangedEvent</c> 事件，由状态机统一驱动。
///     </para>
/// </remarks>
public interface IPokerState : IState
{
    /// <summary>
    ///     本状态对应的状态类型枚举。
    ///     每个 <see cref="IPokerState"/> 实现类对应唯一一个 <see cref="StateType"/>。
    /// </summary>
    StateType StateType { get; set; }

    /// <summary>
    ///     本状态所代理的扑克牌实例。
    ///     状态对象通过此引用读写牌的坐标、外观、动画等属性。
    /// </summary>
    IPoker Poker { get; set; }

    /// <summary>
    ///     请求切换到另一个状态。
    ///     实际切换由状态机订阅 <c>PokerStateChangedEvent</c> 后调用
    ///     当前状态的 <see cref="IState.Exit"/> 与目标状态的 <see cref="IState.Enter"/> 完成。
    /// </summary>
    /// <param name="toState">目标状态类型</param>
    void ChangeTo(StateType toState);

    /// <summary>
    ///     GUI 输入事件入口（由 Godot 的 <c>_GuiInput</c> 传递）。
    /// </summary>
    /// <param name="inputEvent">原始输入事件，通常是 <see cref="InputEventMouseButton"/> 或 <see cref="InputEventMouseMotion"/> </param>
    void GuiInput(InputEvent inputEvent);

    /// <summary>
    ///     鼠标按下回调。
    /// </summary>
    void MouseDown();

    /// <summary>
    ///     鼠标释放回调。
    /// </summary>
    void MouseUp();

    /// <summary>
    ///     鼠标进入牌面区域回调。
    /// </summary>
    void MouseEnter();

    /// <summary>
    ///     鼠标离开牌面区域回调。
    /// </summary>
    void MouseExit();
}