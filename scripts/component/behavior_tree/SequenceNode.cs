using System;
using System.Collections.Generic;

namespace GFrameworkTemplate.scripts.component.behavior_tree;

/// <summary>
///     顺序节点（Sequence）：从左到右依次执行子节点。
///     遇到失败立即整体失败；全部成功才整体成功；遇到执行中则暂停，下一帧从该子节点继续。
///     <para>典型用法：<b>"先做 A，再做 B，最后做 C"</b>（其中一步失败则全不做）。</para>
/// </summary>
/// <example>
///     <code>
///     new SequenceNode(
///         new ConditionNode(() =&gt; hasAmmo),   // 闸门：没弹药整体失败
///         new ActionNode(Reload),               // 装弹
///         new ActionNode(Shoot));               // 射击
///     </code>
/// </example>
public sealed class SequenceNode : BehaviorNode
{
    private readonly IReadOnlyList<BehaviorNode> _children;
    private int _currentIndex;

    /// <summary>
    ///     创建一个顺序节点。
    /// </summary>
    /// <param name="children">按顺序执行的子节点</param>
    public SequenceNode(params BehaviorNode[] children)
    {
        _children = children ?? throw new ArgumentNullException(nameof(children));
    }

    /// <inheritdoc />
    public override NodeStatus Execute()
    {
        // 从上次暂停的位置继续（避免每帧从头重跑已完成/进行中的子节点）
        while (_currentIndex < _children.Count)
        {
            var status = _children[_currentIndex].Execute();

            if (status == NodeStatus.Failure)
            {
                // 某一步失败，整体失败，下次从头开始
                _currentIndex = 0;
                return NodeStatus.Failure;
            }

            if (status == NodeStatus.Running)
            {
                // 当前子节点执行中，记住位置，下一帧继续
                return NodeStatus.Running;
            }

            // 当前子节点成功，进入下一个
            _currentIndex++;
        }

        // 全部成功
        _currentIndex = 0;
        return NodeStatus.Success;
    }
}
