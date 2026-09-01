using System;

namespace GFrameworkTemplate.scripts.component.behavior_tree;

/// <summary>
///     条件节点：判断某个条件是否成立，是行为树的叶子节点。
///     与动作节点的区别：条件节点只做判断、不产生副作用，通常放在序列开头做"闸门"。
/// </summary>
/// <example>
///     <code>
///     new ConditionNode(() =&gt; hasAmmo);
///     </code>
/// </example>
public sealed class ConditionNode : BehaviorNode
{
    private readonly Func<bool> _condition;

    /// <summary>
    ///     创建一个条件节点。
    /// </summary>
    /// <param name="condition">条件判断函数，满足返回 true</param>
    public ConditionNode(Func<bool> condition)
    {
        _condition = condition ?? throw new ArgumentNullException(nameof(condition));
    }

    /// <inheritdoc />
    public override NodeStatus Execute() => _condition() ? NodeStatus.Success : NodeStatus.Failure;
}
