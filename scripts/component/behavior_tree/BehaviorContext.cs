namespace GFrameworkTemplate.scripts.component.behavior_tree;

/// <summary>
///     行为树执行上下文：为节点提供每帧时间信息与共享数据黑板。
/// </summary>
/// <remarks>
///     纯逻辑类，零 Godot / GFramework 依赖，可独立单元测试。
///     由根节点 <see cref="BehaviorTree" /> 持有并逐帧更新 <see cref="Delta" />，
///     再传递给整棵子树，因此节点可以写出"等待 N 秒""冷却"这类与时间相关的行为。
/// </remarks>
public sealed class BehaviorContext
{
    /// <summary>
    ///     获取或设置距上一帧的时间（秒）。
    /// </summary>
    /// <remarks>由根节点在每次 Tick 前写入，节点只读使用。</remarks>
    public double Delta { get; set; }

    /// <summary>
    ///     获取共享数据黑板（同一棵树内所有节点共用）。
    /// </summary>
    public Blackboard Blackboard { get; } = new();
}
