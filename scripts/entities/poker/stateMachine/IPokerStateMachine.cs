using Godot;
using TimeToTwentyfour.scripts.component.stateMachine;
using TimeToTwentyfour.scripts.entities.poker.state;
using TimeToTwentyfour.scripts.enums.poker;

namespace TimeToTwentyfour.scripts.entities.poker.stateMachine;

/// <summary>
///     扑克牌状态机契约
///     定义一张扑克牌在其所有 <see cref="StateType"/> 之间切换与分发的顶层接口。
///     继承 <see cref="IStateMachine"/>，提供面向"单张牌"的专用初始化与输入代理。
/// </summary>
/// <remarks>
///     <para>
///         状态机持有一组 <see cref="IPokerState"/> 实现。
///         运行时仅有一个为活跃状态。所有 GUI 输入与 <c>_Process</c> 帧更新均由状态机接收并转发给当前活跃状态。
///     </para>
///     <para>典型生命周期：</para>
///     <code>
///         // 1. 创建状态机并注入牌实例
///         stateMachine.Init(poker);
///
///         // 2. 初始状态
///         stateMachine.ChangeTo(StateType.Idle);
///
///         // 3. 每帧由牌根节点驱动
///         stateMachine.Process(delta);
///         stateMachine.GuiInput(inputEvent);
///     </code>
/// </remarks>
public interface IPokerStateMachine : IStateMachine
{
     /// <summary>
    ///     初始化状态机，绑定目标扑克牌实例。
    /// </summary>
    /// <param name="poker">
    ///     要代理的扑克牌实例。
    ///     状态机会将该引用注入到每一个 <see cref="IPokerState.Poker"/> 中，
    ///     使各状态能读写牌的坐标、外观、动画等属性。
    /// </param>
    void Init(IPokerView poker);

    /// <summary>
    ///     切换当前活跃状态到指定目标状态。
    /// </summary>
    /// <param name="state">
    ///     目标状态枚举。
    ///     状态机内部会执行 <c>current.Exit() → next.Enter()</c> 流程。
    /// </param>
    /// <remarks>
    ///     <list type="bullet">
    ///         <item>
    ///             若目标状态与当前状态相同，行为由具体实现决定
    ///             （通常忽略或重新触发 <c>Exit/Enter</c>）。
    ///         </item>
    ///         <item>
    ///             切换后，所有后续输入与帧更新将自动路由到新状态。
    ///         </item>
    ///     </list>
    /// </remarks>
    void ChangeTo(StateType state);

    /// <summary>
    ///     GUI 输入事件入口。
    ///     由牌根节点的 <c>_GuiInput</c> 调用，直接转发给当前活跃的 <see cref="IPokerState"/>。
    /// </summary>
    /// <param name="inputEvent"> Godot 原始输入事件。 </param>
    void GuiInput(InputEvent inputEvent);

    /// <summary>
    ///     鼠标按下回调。
    ///     代理到当前活跃状态的 <see cref="IPokerState.MouseDown"/>。
    /// </summary>
    void MouseDown();

    /// <summary>
    ///     鼠标释放回调。
    ///     代理到当前活跃状态的 <see cref="IPokerState.MouseUp"/>。
    /// </summary>
    void MouseUp();

    /// <summary>
    ///     鼠标进入牌面区域回调。
    ///     代理到当前活跃状态的 <see cref="IPokerState.MouseEnter"/>。
    /// </summary>
    void MouseEnter();

    /// <summary>
    ///     鼠标离开牌面区域回调。
    ///     代理到当前活跃状态的 <see cref="IPokerState.MouseExit"/>。
    /// </summary>
    void MouseExit();
}