using Godot;
using TimeToTwentyfour.scripts.component.state_machine;
using TimeToTwentyfour.scripts.enums.poker;

namespace TimeToTwentyfour.scripts.entities.poker.state;

/// <summary>
///     扑克状态契约
///     定义一张扑克在特定 <see cref="PokerStateType"/> 下的交互行为与生命周期。
///     继承 <see cref="IState"/>，可被 <see cref="IPokerStateMachine"/> 统一管理。
/// </summary>
/// <remarks>
///     <para>
///         一张扑克在运行时会处于下列状态之一：
///     </para>
///     <list type="table">
///         <listheader><term>状态</term><description>说明</description></listheader>
///         <item><term><see cref="PokerStateType.Idle"/></term><description>闲置</description></item>
///         <item><term><see cref="PokerStateType.Drag"/></term><description>拖动</description></item>
///         <item><term><see cref="PokerStateType.UnSelect"/></term><description>未选中</description></item>
///         <item><term><see cref="PokerStateType.OnSelect"/></term><description>被选中</description></item>
///     </list>
///     <para>
///         状态切换由 <see cref="PokerState.ChangeTo"/> 直接委托给 <see cref="IPoker.ChangeTo"/>
///         → <see cref="IPokerStateMachine.ChangeTo"/>，执行 <c>Exit/Enter</c> 流程。
///     </para>
/// </remarks>
public interface IPokerState : IState
{
    /// <summary>
    ///     本状态对应的状态类型枚举。
    ///     每个 <see cref="IPokerState"/> 实现类对应唯一一个 <see cref="PokerStateType"/>。
    /// </summary>
    PokerStateType PokerStateType { get; set; }

    /// <summary>
    ///     本状态所代理的扑克牌实例。
    ///     状态对象通过此引用读写牌的坐标、外观、动画等属性。
    /// </summary>
    IPokerView Poker { get; set; }

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