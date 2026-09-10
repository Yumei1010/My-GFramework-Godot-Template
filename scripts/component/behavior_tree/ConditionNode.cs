using GFramework.Core.SourceGenerators.Abstractions.Logging;
using Godot;
using GFrameworkTemplate.scripts.enums.behavior_tree;

namespace GFrameworkTemplate.scripts.component.behavior_tree;

/// <summary>
///     条件节点：判断某个条件是否成立，是行为树的叶子节点。
///     与动作节点的区别：条件节点只做判断、不产生副作用，通常放在序列开头做"闸门"。
///     支持两种绑定方式（二选一）：
///     <list type="bullet">
///         <item><description>在编辑器中把 <see cref="Condition"/> 设为任意节点的某个方法（返回 bool）</description></item>
///         <item><description>通过 <see cref="SetCondition"/> 注入 C# 委托</description></item>
///     </list>
/// </summary>
/// <example>
///     <code>
///     var hasAmmo = new ConditionNode();
///     hasAmmo.SetCondition(() =&gt; ammo &gt; 0);
///     </code>
/// </example>
[Log]
public partial class ConditionNode : BehaviorNode
{
    private Func<bool>? _delegateCondition;
    private Func<BehaviorContext, bool>? _contextCondition;

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

    /// <summary>
    ///     通过带执行上下文的委托注入条件判断（可读取 <see cref="BehaviorContext.Delta" /> 与黑板）。
    /// </summary>
    /// <param name="condition">条件委托，接收执行上下文并返回是否满足</param>
    public void SetCondition(Func<BehaviorContext, bool> condition)
    {
        _contextCondition = condition;
    }

    /// <inheritdoc />
    public override NodeStatus Execute(BehaviorContext context)
    {
        if (_contextCondition is not null)
            return _contextCondition(context) ? NodeStatus.Success : NodeStatus.Failure;

        if (_delegateCondition is not null)
            return _delegateCondition() ? NodeStatus.Success : NodeStatus.Failure;

        if (Condition.Method == default)
        {
            _log.Warn($"条件节点 [{Name}] 未配置 Condition 委托或 Callable，返回失败");
            return NodeStatus.Failure;
        }

        return Condition.Call().AsBool() ? NodeStatus.Success : NodeStatus.Failure;
    }
}
