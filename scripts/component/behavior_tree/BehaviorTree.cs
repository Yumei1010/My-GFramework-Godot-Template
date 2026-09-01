namespace GFrameworkTemplate.scripts.component.behavior_tree;

/// <summary>
///     行为树：管理一棵行为树并逐帧驱动其执行。
///     行为树本身是一个根节点，通常根节点用 <see cref="SelectorNode"/>（选一个策略）或 <see cref="SequenceNode"/>（按步骤做事）。
/// </summary>
/// <example>
///     <code>
///     var tree = new BehaviorTree(
///         new SelectorNode(
///             new SequenceNode(
///                 new ConditionNode(() =&gt; hasTarget),
///                 new ActionNode(Attack)),
///             new ActionNode(Patrol)));
///
///     tree.Start();
///     // _Process 里每帧：
///     tree.Tick();
///     </code>
/// </example>
public sealed class BehaviorTree
{
    private readonly BehaviorNode _root;

    /// <summary>
    ///     上一次根节点执行的结果；未执行过时为 null。
    /// </summary>
    public NodeStatus? LastStatus { get; private set; }

    /// <summary>
    ///     创建一个行为树。
    /// </summary>
    /// <param name="root">根节点（通常为复合节点）</param>
    public BehaviorTree(BehaviorNode root)
    {
        _root = root ?? throw new ArgumentNullException(nameof(root));
    }

    /// <summary>
    ///     执行一帧：从根节点开始评估整棵树。
    ///     应在每帧的更新逻辑中调用。
    /// </summary>
    /// <returns>根节点执行结果</returns>
    public NodeStatus Tick()
    {
        LastStatus = _root.Execute();
        return LastStatus.Value;
    }
}
