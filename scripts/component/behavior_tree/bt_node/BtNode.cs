using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using Godot;

namespace GFrameworkTemplate.scripts.component.behavior_tree.bt_node;

/// <summary>
///     Godot 可视化行为树节点的抽象基类。
///     每个行为树节点都是一个 Godot 节点，在场景树中拼装层级即组成行为树：
///     复合节点（<see cref="BtSequenceNode"/> / <see cref="BtSelectorNode"/>）的子节点即其执行序列。
/// </summary>
/// <remarks>
///     纯逻辑版见 <c>scripts/component/behavior_tree/</c>（不依赖 Godot）；本节点版用于在编辑器中可视化拼装。
/// </remarks>
[Log]
[ContextAware]
public abstract partial class BtNode : Node
{
    /// <summary>
    ///     当前节点的子行为树节点（复合节点用）。
    /// </summary>
    protected IReadOnlyList<BtNode> ChildNodes => GetChildren().OfType<BtNode>().ToList();

    /// <summary>
    ///     执行本节点，返回执行结果。
    ///     <para>复合节点执行时会依次执行子节点；返回 <see cref="NodeStatus.Running"/> 表示任务需多帧完成。</para>
    /// </summary>
    /// <returns>成功 / 失败 / 执行中</returns>
    public abstract NodeStatus Execute();

    /// <summary>
    ///     把 Godot 方法的返回值转换为行为树执行结果。
    ///     <list type="bullet">
    ///         <item><description>int：按 <see cref="NodeStatus"/> 枚举值解析（0 成功 / 1 失败 / 2 执行中）</description></item>
    ///         <item><description>bool：true 成功 / false 失败</description></item>
    ///         <item><description>无返回值（void）：视为成功</description></item>
    ///     </list>
    /// </summary>
    /// <param name="result">Godot 方法调用返回值</param>
    /// <returns>行为树执行结果</returns>
    protected static NodeStatus ToStatus(Variant result)
    {
        switch (result.VariantType)
        {
            case Variant.Type.Int:
                var value = result.AsInt32();
                return Enum.IsDefined(typeof(NodeStatus), value) ? (NodeStatus)value : NodeStatus.Failure;
            case Variant.Type.Bool:
                return result.AsBool() ? NodeStatus.Success : NodeStatus.Failure;
            case Variant.Type.Nil:
            default:
                return NodeStatus.Success;
        }
    }
}
