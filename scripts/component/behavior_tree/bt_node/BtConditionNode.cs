using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using Godot;

namespace GFrameworkTemplate.scripts.component.behavior_tree.bt_node;

/// <summary>
///     行为树条件节点（Godot 节点版）：判断条件是否成立，通常放在序列开头做"闸门"。
///     支持两种绑定方式（二选一）：
///     <list type="bullet">
///         <item><description>在编辑器中把 <see cref="Condition"/> 设为任意节点的某个方法（返回 bool）</description></item>
///         <item><description>通过 <see cref="SetCondition"/> 注入 C# 委托</description></item>
///     </list>
/// </summary>
[Log]
public partial class BtConditionNode : BtNode
{
    private Func<bool>? _delegateCondition;

    /// <summary>
    ///     要调用的 Godot 方法（返回 bool）。
    /// </summary>
    [Export]
    public Callable Condition { get; set; }

    /// <summary>
    ///     通过 C# 委托注入条件判断。
    /// </summary>
    /// <param name="condition">条件委托，满足返回 true</param>
    public void SetCondition(Func<bool> condition)
    {
        _delegateCondition = condition;
    }

    /// <inheritdoc />
    public override NodeStatus Execute()
    {
        if (_delegateCondition is not null)
            return _delegateCondition() ? NodeStatus.Success : NodeStatus.Failure;

        return Condition.Method != default && Condition.Call().AsBool()
            ? NodeStatus.Success
            : NodeStatus.Failure;
    }
}
