using System;
using System.Collections.Generic;

namespace GFrameworkTemplate.scripts.component.behavior_tree;

/// <summary>
///     选择节点（Selector）：从左到右依次尝试子节点，找到第一个成功的执行。
///     遇到成功立即整体成功；全部失败才整体失败；遇到执行中则暂停，下一帧从该子节点继续。
///     <para>典型用法：<b>"优先做 A，不行就做 B，再不行做 C"</b>（回退/兜底逻辑）。</para>
/// </summary>
/// <example>
///     <code>
///     new SelectorNode(
///         new ActionNode(AttackTarget),  // 优先：攻击
///         new ActionNode(ChaseTarget),   // 回退：追目标
///         new ActionNode(Patrol));       // 兜底：巡逻
///     </code>
/// </example>
public sealed class SelectorNode : BehaviorNode
{
    private readonly IReadOnlyList<BehaviorNode> _children;
    private int _currentIndex;

    /// <summary>
    ///     创建一个选择节点。
    /// </summary>
    /// <param name="children">按优先级从高到低尝试的子节点</param>
    public SelectorNode(params BehaviorNode[] children)
    {
        _children = children ?? throw new ArgumentNullException(nameof(children));
    }

    /// <inheritdoc />
    public override NodeStatus Execute()
    {
        // 从上次暂停的位置继续
        while (_currentIndex < _children.Count)
        {
            var status = _children[_currentIndex].Execute();

            if (status == NodeStatus.Success)
            {
                // 找到一个成功的，整体成功，下次从头开始
                _currentIndex = 0;
                return NodeStatus.Success;
            }

            if (status == NodeStatus.Running)
            {
                // 当前子节点执行中，记住位置，下一帧继续
                return NodeStatus.Running;
            }

            // 当前子节点失败，尝试下一个
            _currentIndex++;
        }

        // 全部失败
        _currentIndex = 0;
        return NodeStatus.Failure;
    }
}
