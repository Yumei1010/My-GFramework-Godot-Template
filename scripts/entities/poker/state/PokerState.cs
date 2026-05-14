using Godot;
using TimeToTwentyfour.scripts.enums.poker;

namespace TimeToTwentyfour.scripts.entities.poker.state;

/// <summary>
///     扑克牌状态抽象基类。
///     为所有 <see cref="IPokerState"/> 提供默认空实现，子类仅覆写所需方法。
/// </summary>
/// <remarks>
///     <para>
///         子类只需关注该状态下每个输入该如何响应：
///     </para>
///     <code>
///         public sealed class UnSelectState : PokerState
///         {
///             public override void MouseDown()
///             {
///                 // 选中 → 切换到 OnSelect 状态
///                 ChangeTo(StateType.OnSelect);
///             }
///         }
///     </code>
/// </remarks>
public abstract class PokerState : IPokerState
{
    /// <summary>
    ///     本状态对应的 <see cref="StateType"/> 枚举值。
    /// </summary>
    public StateType StateType { get; set; }

    /// <summary>
    ///     本状态所代理的扑克牌实例。
    ///     初始化时由状态机注入。
    /// </summary>
    public IPokerView Poker { get; set; }

    /// <summary>
    ///     请求切换到另一个状态，直接委托给 <see cref="IPoker.ChangeTo"/> → <see cref="IPokerStateMachine.ChangeTo"/>。
    /// </summary>
    /// <param name="state">目标状态类型。</param>
    protected void ChangeTo(StateType state)
    {
        Poker.ChangeTo(state);
    }

    /// <inheritdoc />
    public virtual void GuiInput(InputEvent inputEvent) { }

    /// <inheritdoc />
    public virtual void MouseDown() { }

    /// <inheritdoc />
    public virtual void MouseUp() { }

    /// <inheritdoc />
    public virtual void MouseEnter() { }

    /// <inheritdoc />
    public virtual void MouseExit() { }

    /// <inheritdoc />
    public virtual void Process(double delta) { }

    /// <inheritdoc />
    public virtual void Enter() { }

    /// <inheritdoc />
    public virtual void Exit() { }
}
