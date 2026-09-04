using GFramework.Godot.Pool;
using Godot;

namespace GFrameworkTemplate.scripts.component.object_pool;

/// <summary>
///     示例对象池系统：子弹节点池。
///     继承框架 <see cref="AbstractNodePoolSystem{TKey, TNode}"/>（方案 A1：每对象类型一个池系统类），
///     通过架构注册为 System。键为场景标识字符串，场景资源在 <see cref="LoadScene"/> 中加载。
/// </summary>
/// <remarks>
///     注册（SystemModule）：
///     <code>
///     architecture.RegisterSystem(new BulletPoolSystem());
///     </code>
///     使用（任意 [ContextAware] 节点）：
///     <code>
///     var pool = this.GetSystem&lt;BulletPoolSystem&gt;()!;
///     var bullet = pool.Acquire(BulletPoolSystem.Key);   // 取（复用或新实例化）
///     GetNode&lt;Node2D&gt;("%Bullets").AddChild(bullet);      // 挂到场景
///     bullet.Fire(pos, dir);                             // 发射
///     // ... 命中/出界后归还：
///     bullet.GetParent()?.RemoveChild(bullet);           // 从场景摘下
///     pool.Release(BulletPoolSystem.Key, bullet);        // 归还池（下次复用）
///     </code>
/// </remarks>
public class BulletPoolSystem : AbstractNodePoolSystem<string, BulletNode>
{
    /// <summary>
    ///     池键（示例固定；真实项目可用枚举/常量区分多场景）。
    /// </summary>
    public const string Key = "bullet";

    /// <summary>
    ///     子弹场景路径。
    /// </summary>
    private const string ScenePath = "res://scenes/objects/bullet.tscn";

    /// <inheritdoc />
    protected override void OnInit()
    {
        // 可在此 Prewarm 预热：Prewarm(Key, 10);
    }

    /// <summary>
    ///     加载子弹场景。
    /// </summary>
    /// <param name="key">池键（本示例固定 Key）</param>
    /// <returns>子弹 PackedScene</returns>
    protected override PackedScene LoadScene(string key)
    {
        return GD.Load<PackedScene>(ScenePath);
    }
}
