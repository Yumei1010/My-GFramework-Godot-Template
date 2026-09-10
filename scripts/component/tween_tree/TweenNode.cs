using GFramework.Core.SourceGenerators.Abstractions.Logging;
using GFramework.Core.SourceGenerators.Abstractions.Rule;
using Godot;

namespace GFrameworkTemplate.scripts.component.tween_tree;

/// <summary>
///     Tween 动画树节点抽象基类。
///     每个节点是一个 Godot 节点，在场景树中拼装层级即组成一条动画序列：
///     叶子节点（属性动画/延时）做具体动画，组合节点（顺序/并行）编排子节点。
/// </summary>
/// <remarks>
///     <para>父节点执行时通过 <see cref="Tween.TweenSubtween"/> 嵌入子节点的 Tween（引擎自动接管子 Tween）。</para>
///     <para>整棵树由 <see cref="TweenTree"/> 根节点驱动播放。</para>
/// </remarks>
[Log]
[ContextAware]
public abstract partial class TweenNode : Node
{
    private IReadOnlyList<TweenNode>? _children;

    /// <summary>
    ///     当前节点的子动画节点（组合节点用）。
    /// </summary>
    /// <remarks>
    ///     结果会被缓存：子节点增删（<see cref="Node.NotificationChildOrderChanged" />）时自动失效。
    /// </remarks>
    protected IReadOnlyList<TweenNode> ChildNodes =>
        _children ??= GetChildren().OfType<TweenNode>().ToList();

    /// <summary>
    ///     使子节点缓存立即失效（子节点增删会自动失效，一般无需手动调用）。
    /// </summary>
    public void InvalidateChildren()
    {
        _children = null;
    }

    /// <inheritdoc />
    public override void _Notification(int what)
    {
        if (what == NotificationChildOrderChanged)
        {
            InvalidateChildren();
        }
    }

    /// <summary>
    ///     构建本节点的 Tween 片段（供父节点嵌入或根节点播放）。
    /// </summary>
    /// <remarks>
    ///     每个节点独立创建 Tween（<c>CreateTween()</c>），父节点用 <c>TweenSubtween</c> 嵌入时
    ///     引擎会自动把子 Tween 从 SceneTree 移除并接管其执行。
    /// </remarks>
    /// <returns>本节点的 Tween 片段</returns>
    public abstract Tween BuildTween();

    /// <summary>
    ///     播放本节点（独立运行，不依赖父节点——便于调试单个叶子）。
    /// </summary>
    /// <returns>本节点的 Tween</returns>
    public Tween Play()
    {
        var tween = BuildTween();
        return tween;
    }
}
