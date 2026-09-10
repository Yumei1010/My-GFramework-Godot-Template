using GFramework.Core.SourceGenerators.Abstractions.Logging;
using Godot;
using GFrameworkTemplate.scripts.enums.behavior_tree;

namespace GFrameworkTemplate.scripts.component.behavior_tree;

/// <summary>
///     等待节点：持续 <see cref="Duration" /> 秒后返回成功，期间返回执行中。
/// </summary>
/// <remarks>
///     演示执行上下文的用途：通过 <see cref="BehaviorContext.Delta" /> 累计时间，
///     因此天然受游戏时间缩放/暂停影响（配合 pause 系统可整体冻结）。
///     典型用法：`Sequence(动作A, WaitNode(1.0), 动作B)` —— 动作之间插入停顿。
/// </remarks>
[Log]
public partial class WaitNode : BehaviorNode
{
    /// <summary>
    ///     等待时长（秒）。
    /// </summary>
    [Export(PropertyHint.Range, "0.01,60,0.01")]
    public float Duration { get; set; } = 1f;

    private double _remaining;
    private bool _waiting;

    /// <summary>
    ///     重置计时（下次执行时重新开始等待）。
    /// </summary>
    public void ResetWait()
    {
        _waiting = false;
        _remaining = 0;
    }

    /// <inheritdoc />
    public override NodeStatus Execute(BehaviorContext context)
    {
        if (!_waiting)
        {
            _waiting = true;
            _remaining = Duration;
        }

        _remaining -= context.Delta;

        if (_remaining > 0)
        {
            return NodeStatus.Running;
        }

        _waiting = false;
        return NodeStatus.Success;
    }
}
