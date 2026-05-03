using GFramework.Core.extensions;
using GFramework.SourceGenerators.Abstractions.rule;
using Godot;
using TimeToTwentyfour.scripts.cqrs.poker.@event;
using TimeToTwentyfour.scripts.enums.poker;

namespace TimeToTwentyfour.scripts.entities.poker.state;

/// <summary>
///     扑克牌状态契约抽象实现基类
///     负责封装所有 <see cref="IPokerState"/> 共用的事件发送逻辑与字段声明。
/// </summary>
/// <remarks>
///     <para>
///             子类只需关注该状态下每个输入该如何响应：
///     </para>
///     <code>
///         public partial class UnSelectState : PokerState
///         {
///             public override void MouseDown()
///             {
///                 // 选中 → 切换到 Selected 状态
///                 ChangeTo(StateType.OnSelect);
///             }
///
///             public override void MouseUp()   { /* 无操作 */ }
///             public override void MouseEnter() { Poker.Shadow = true;  }
///             public override void MouseExit()  { Poker.Shadow = false; }
///             public override void GuiInput(InputEvent e) { /* 分发到 MouseDown/Up */ }
///             public override void Process(double delta) { /* 每帧逻辑 */ }
///             public override void Enter()  { /* 进入状态时的初始化 */ }
///             public override void Exit()   { /* 离开状态时的清理 */ }
///         }
///     </code>
/// </remarks>
[ContextAware]
public abstract partial class PokerState : Node, IPokerState
{
    /// <summary>
    ///     本状态对应的 <see cref="StateType"/> 枚举值。
    /// </summary>
    [Export] public StateType StateType { get; set; }

    /// <summary>
    ///     本状态所代理的扑克牌实例。
    ///     初始化时由状态机注入，永远不为 <c>null</c>（<c>null!</c> 保证）。
    /// </summary>
    public IPoker Poker { get; set; } = null!;

    /// <summary>
    ///     请求切换到另一个状态。
    /// </summary>
    /// <param name="state">目标状态类型。</param>
    /// <remarks>
    ///     <para>
    ///         通过发出<see cref="PokerStateChangedEvent"/>。
    ///         该事件携带当前状态实例 (<c>State = this</c>) 与目标状态枚举 (<c>TargetState</c>)，
    ///         由状态机监听并统一执行 <c>currentState.Exit() → nextState.Enter()</c> 流程。
    ///     </para>
    /// </remarks>
    public void ChangeTo(StateType state)
    {
        ContextAwareExtensions.SendEvent(this, new PokerStateChangedEvent
        {
            TargetState = state,
            State = this
        });
    }

    /// <summary>
    ///     GUI 输入事件入口（具体行为由子类定义）。
    /// </summary>
    /// <param name="inputEvent">Godot 传递的原始输入事件。</param>
    public abstract void GuiInput(InputEvent inputEvent);

    /// <summary>
    ///     鼠标按下时的响应逻辑（具体行为由子类定义）。
    /// </summary>
    public abstract void MouseDown();

    /// <summary>
    ///     鼠标释放时的响应逻辑（具体行为由子类定义）。
    /// </summary>
    public abstract void MouseUp();

    /// <summary>
    ///     鼠标进入牌面区域时的响应逻辑（具体行为由子类定义）。
    /// </summary>
    public abstract void MouseEnter();

    /// <summary>
    ///     鼠标离开牌面区域时的响应逻辑（具体行为由子类定义）。
    /// </summary>
    public abstract void MouseExit();

    /// <summary>
    ///     每帧更新回调（具体行为由子类定义）。
    ///     由状态机在 <c>_Process</c> 中驱动，仅对当前活跃状态调用。
    /// </summary>
    /// <param name="delta">上一帧到本帧的时间间隔（秒）。</param>
    public abstract void Process(double delta);

    /// <summary>
    ///     进入本状态时调用（具体行为由子类定义）。
    /// </summary>
    public abstract void Enter();

    /// <summary>
    ///     离开本状态时调用（具体行为由子类定义）。
    /// </summary>
    public abstract void Exit();
}