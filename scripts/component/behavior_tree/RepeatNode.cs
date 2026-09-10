using GFramework.Core.SourceGenerators.Abstractions.Logging;
using Godot;
using GFrameworkTemplate.scripts.enums.behavior_tree;

namespace GFrameworkTemplate.scripts.component.behavior_tree;

/// <summary>
///     重复节点：反复执行唯一子节点，直到达到指定次数。
/// </summary>
/// <remarks>
///     <see cref="Times" /> 大于 0 时重复该次数；小于等于 0 时无限重复。
///     子节点失败会立即中止并重置计数；子节点执行中则原样透传执行中。
///     典型用法：`RepeatNode(3) → 挥砍动作` —— 连续挥砍三次。
/// </remarks>
[Log]
public partial class RepeatNode : BehaviorNode
{
    /// <summary>
    ///     重复次数；小于等于 0 表示无限重复。
    /// </summary>
    [Export]
    public int Times { get; set; } = 3;

    private int _completed;

    /// <summary>
    ///     重置已完成计数（下次执行时从头开始计）。
    /// </summary>
    public void ResetRepeat()
    {
        _completed = 0;
    }

    /// <inheritdoc />
    public override NodeStatus Execute(BehaviorContext context)
    {
        var child = ChildNodes.FirstOrDefault();
        if (child is null)
        {
            _log.Warn($"重复节点 [{Name}] 缺少子节点，返回失败");
            return NodeStatus.Failure;
        }

        var status = child.Execute(context);
        if (status == NodeStatus.Running)
        {
            return NodeStatus.Running;
        }

        if (status == NodeStatus.Failure)
        {
            _completed = 0;
            return NodeStatus.Failure;
        }

        _completed++;
        if (Times > 0 && _completed >= Times)
        {
            _completed = 0;
            return NodeStatus.Success;
        }

        // 还需继续重复：本轮返回执行中，下一帧再次执行子节点
        return NodeStatus.Running;
    }
}
