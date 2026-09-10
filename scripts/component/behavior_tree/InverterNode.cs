using GFramework.Core.SourceGenerators.Abstractions.Logging;
using GFrameworkTemplate.scripts.enums.behavior_tree;

namespace GFrameworkTemplate.scripts.component.behavior_tree;

/// <summary>
///     取反装饰器节点：把唯一子节点的成功与失败互换（执行中保持不变）。
/// </summary>
/// <remarks>
///     典型用法："条件不满足才执行"——`InverterNode → ConditionNode(有目标?)`
///     会在"没有目标"时返回成功，从而作为 SequenceNode 的闸门。
/// </remarks>
[Log]
public partial class InverterNode : BehaviorNode
{
    /// <inheritdoc />
    public override NodeStatus Execute(BehaviorContext context)
    {
        var child = ChildNodes.FirstOrDefault();
        if (child is null)
        {
            _log.Warn($"取反节点 [{Name}] 缺少子节点，返回失败");
            return NodeStatus.Failure;
        }

        return child.Execute(context) switch
        {
            NodeStatus.Success => NodeStatus.Failure,
            NodeStatus.Failure => NodeStatus.Success,
            _ => NodeStatus.Running,
        };
    }
}
