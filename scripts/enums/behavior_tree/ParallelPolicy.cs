namespace GFrameworkTemplate.scripts.enums.behavior_tree;

/// <summary>
///     并行节点的成功判定策略。
/// </summary>
public enum ParallelPolicy
{
    /// <summary>
    ///     全部子节点成功才算成功（任一失败即整体失败）。
    /// </summary>
    RequireAll,

    /// <summary>
    ///     任一子节点成功即算成功（全部失败才整体失败）。
    /// </summary>
    RequireOne,
}
