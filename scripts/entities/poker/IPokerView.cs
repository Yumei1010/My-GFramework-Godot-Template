using Godot;
using TimeToTwentyfour.scripts.enums.poker;

namespace TimeToTwentyfour.scripts.entities.poker;

/// <summary>
///     扑克牌视图层契约
///     仅包含扑克的视觉属性与空间操作，不涉及牌面数据。
/// </summary>
public interface IPokerView
{
    /// <summary>唯一标识符，用于 CQRS 事件匹配。</summary>
    Guid Id { get; set; }

    /// <summary>是否启用阴影投影。</summary>
    bool Shadow { get; set; }

    /// <summary>是否开启动画过渡。</summary>
    bool Animate { get; set; }

    /// <summary>单次动画持续时间（秒）。</summary>
    float AnimateTime { get; set; }

    /// <summary>是否启用伪 3D 透视变形。</summary>
    bool Fake3D { get; set; }

    /// <summary>是否置顶渲染（脱离父级变换）。</summary>
    bool TopLevel { get; set; }

    /// <summary>牌面尺寸。</summary>
    Vector2 Size { get; }

    /// <summary>全局坐标。</summary>
    Vector2 GlobalPosition { get; set; }

    /// <summary>旋转弧度。</summary>
    float Rotation { get; }

    /// <summary>重置位置锚点。</summary>
    Vector2 ResetPosition { get; set; }

    /// <summary>重置旋转锚点。</summary>
    float ResetRotation { get; set; }

    /// <summary>获取鼠标全局位置。</summary>
    Vector2 GetGlobalMousePosition();

    /// <summary>获取父节点。</summary>
    Node GetParent();

    /// <summary>更换父节点。</summary>
    void Reparent(Node parent);

    /// <summary>放置到指定位置（无动画）。</summary>
    void SpawnTo(Vector2 position);

    /// <summary>移动到指定位置（根据 <see cref="Animate"/> 决定动画）。</summary>
    void MoveTo(Vector2 position);

    /// <summary>重置指定属性到锚点值。</summary>
    void Reset(string attributeName);

    /// <summary>变更到指定状态。</summary>
    void ChangeTo(StateType state);
}
