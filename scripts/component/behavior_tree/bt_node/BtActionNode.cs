using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using Godot;

namespace GFrameworkTemplate.scripts.component.behavior_tree.bt_node;

/// <summary>
///     行为树动作节点（Godot 节点版）：执行一个具体动作。
///     支持两种绑定方式（二选一）：
///     <list type="bullet">
///         <item><description>在编辑器中把 <see cref="Action"/> 设为任意节点的某个方法（返回 int 状态 / bool / void）</description></item>
///         <item><description>通过 <see cref="SetAction"/> 注入 C# 委托</description></item>
///     </list>
/// </summary>
[Log]
public partial class BtActionNode : BtNode
{
    private Func<NodeStatus>? _delegateAction;

    /// <summary>
    ///     要调用的 Godot 方法（可在编辑器绑定，或代码调用 <see cref="SetAction"/> 注入委托）。
    /// </summary>
    [Export]
    public Callable Action { get; set; }

    /// <summary>
    ///     通过 C# 委托注入动作逻辑。
    /// </summary>
    /// <param name="action">动作委托，返回执行结果（成功 / 失败 / 执行中）</param>
    public void SetAction(Func<NodeStatus> action)
    {
        _delegateAction = action;
    }

    /// <inheritdoc />
    public override NodeStatus Execute()
    {
        if (_delegateAction is not null)
            return _delegateAction();

        return Action.Method != default ? ToStatus(Action.Call()) : NodeStatus.Failure;
    }
}
