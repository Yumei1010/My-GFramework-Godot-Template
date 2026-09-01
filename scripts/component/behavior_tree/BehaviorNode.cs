namespace GFrameworkTemplate.scripts.component.behavior_tree;

/// <summary>
///     行为树节点的抽象基类。
///     节点是行为树的基本单元：叶子节点（动作/条件）做具体事，复合节点（顺序/选择）控制执行逻辑。
/// </summary>
public abstract class BehaviorNode
{
    /// <summary>
    ///     执行本节点，返回执行结果。
    /// </summary>
    /// <remarks>
    ///     复合节点执行时会递归调用子节点，整棵树从根节点开始自上而下评估。
    ///     <para>返回 <see cref="NodeStatus.Running"/> 表示任务需要多帧完成，下一帧会继续执行。</para>
    /// </remarks>
    /// <returns>节点执行结果：成功 / 失败 / 执行中</returns>
    public abstract NodeStatus Execute();
}
