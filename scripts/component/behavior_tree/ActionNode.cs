using System;

namespace GFrameworkTemplate.scripts.component.behavior_tree;

/// <summary>
///     动作节点：执行一个具体动作，是行为树的叶子节点。
/// </summary>
/// <example>
///     例如"攻击目标"、"走向门口"、"播放动画" 都是动作：
///     <code>
///     new ActionNode(() =&gt;
///     {
///         if (IsTargetInRange()) return NodeStatus.Success;
///         MoveToward(target);
///         return NodeStatus.Running; // 正在移动，下一帧继续
///     });
///     </code>
/// </example>
public sealed class ActionNode : BehaviorNode
{
    private readonly Func<NodeStatus> _action;

    /// <summary>
    ///     创建一个动作节点。
    /// </summary>
    /// <param name="action">要执行的动作，返回执行结果（成功 / 失败 / 执行中）</param>
    public ActionNode(Func<NodeStatus> action)
    {
        _action = action ?? throw new ArgumentNullException(nameof(action));
    }

    /// <inheritdoc />
    public override NodeStatus Execute() => _action();
}
