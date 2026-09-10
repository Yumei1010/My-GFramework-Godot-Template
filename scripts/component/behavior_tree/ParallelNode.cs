using GFramework.Core.SourceGenerators.Abstractions.Logging;
using Godot;
using GFrameworkTemplate.scripts.enums.behavior_tree;

namespace GFrameworkTemplate.scripts.component.behavior_tree;

/// <summary>
///     并行节点：同一帧内驱动**全部**子节点（区别于顺序/选择节点的逐个推进）。
///     判定策略由 <see cref="SuccessPolicy" /> 决定：
///     <list type="bullet">
///         <item><description><see cref="ParallelPolicy.RequireAll" />（默认）：全部成功才成功，任一失败即失败</description></item>
///         <item><description><see cref="ParallelPolicy.RequireOne" />：任一成功即成功，全部失败才失败</description></item>
///     </list>
/// </summary>
/// <remarks>
///     有子节点处于 <see cref="NodeStatus.Running" /> 且尚未判定成败时，整体返回执行中。
///     典型用法："边走边射击"（移动 + 攻击两个动作同时推进）。
/// </remarks>
[Log]
public partial class ParallelNode : BehaviorNode
{
    /// <summary>
    ///     成功判定策略的导出值（编辑器下拉选择）。
    /// </summary>
    /// <remarks>
    ///     说明：直接 <c>[Export]</c> 自定义 C# 枚举会在 Godot 退出时留下 Variant 池残留告警，
    ///     因此这里以 <c>int + PropertyHint.Enum</c> 导出，对外仍通过 <see cref="SuccessPolicy" /> 暴露类型安全的枚举。
    /// </remarks>
    [Export(PropertyHint.Enum, "RequireAll,RequireOne")]
    public int SuccessPolicyValue { get; set; } = (int)ParallelPolicy.RequireAll;

    /// <summary>
    ///     获取成功判定策略。
    /// </summary>
    public ParallelPolicy SuccessPolicy => (ParallelPolicy)SuccessPolicyValue;

    /// <inheritdoc />
    public override NodeStatus Execute(BehaviorContext context)
    {
        if (ChildNodes.Count == 0)
        {
            return NodeStatus.Success;
        }

        var successCount = 0;
        var failureCount = 0;
        var runningCount = 0;

        foreach (var child in ChildNodes)
        {
            switch (child.Execute(context))
            {
                case NodeStatus.Success:
                    successCount++;
                    break;
                case NodeStatus.Failure:
                    failureCount++;
                    break;
                default:
                    runningCount++;
                    break;
            }
        }

        if (SuccessPolicy == ParallelPolicy.RequireAll)
        {
            if (failureCount > 0)
            {
                return NodeStatus.Failure;
            }

            return runningCount > 0 ? NodeStatus.Running : NodeStatus.Success;
        }

        if (successCount > 0)
        {
            return NodeStatus.Success;
        }

        return runningCount > 0 ? NodeStatus.Running : NodeStatus.Failure;
    }
}
