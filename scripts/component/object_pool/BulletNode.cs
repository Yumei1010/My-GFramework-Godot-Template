using GFramework.Core.Abstractions.Pool;
using GFramework.Core.SourceGenerators.Abstractions.Logging;
using GFramework.Core.SourceGenerators.Abstractions.Rule;
using GFramework.Godot.Pool;
using Godot;

namespace GFrameworkTemplate.scripts.component.object_pool;

/// <summary>
///     示例池化节点：子弹（用于演示对象池 A1 用法）。
///     池生命周期由框架 <see cref="IPoolableNode"/> 驱动：
///     <see cref="IPoolableObject.OnAcquire"/>（取池复位）/ <see cref="IPoolableObject.OnRelease"/>（归还复位）。
/// </summary>
[Log]
[ContextAware]
public partial class BulletNode : Node2D, IPoolableNode
{
    private bool _flying;

    /// <summary>
    ///     飞行速度（像素/秒）。
    /// </summary>
    [Export] public float Speed { get; set; } = 300f;

    /// <summary>
    ///     当前飞行方向（单位向量）。
    /// </summary>
    public Vector2 Direction { get; private set; }

    /// <summary>
    ///     是否正在飞行。
    /// </summary>
    public bool IsFlying => _flying;

    /// <summary>
    ///     发射：设定方向并开始飞行（由调用方在 Acquire + AddChild 后调用）。
    /// </summary>
    /// <param name="position">出生位置</param>
    /// <param name="direction">飞行方向</param>
    public void Fire(Vector2 position, Vector2 direction)
    {
        GlobalPosition = position;
        Direction = direction.Normalized();
        _flying = true;
        SetPhysicsProcess(true);
        Show();
    }

    /// <summary>
    ///     每帧推进子弹（飞行动画演示）。
    /// </summary>
    public override void _PhysicsProcess(double delta)
    {
        if (!_flying)
            return;
        GlobalPosition += Direction * (Speed * (float)delta);
    }

    /// <inheritdoc />
    void IPoolableObject.OnAcquire()
    {
        // 取池复位：由调用方随后 Fire() 决定位置与发射
        _flying = false;
        Direction = Vector2.Zero;
        SetPhysicsProcess(false);
        Hide();
    }

    /// <inheritdoc />
    void IPoolableObject.OnRelease()
    {
        // 归还复位
        _flying = false;
        SetPhysicsProcess(false);
        Hide();
    }

    /// <inheritdoc />
    void IPoolableObject.OnPoolDestroy()
    {
        QueueFree();
    }

    /// <inheritdoc />
    public Node AsNode() => this;
}
