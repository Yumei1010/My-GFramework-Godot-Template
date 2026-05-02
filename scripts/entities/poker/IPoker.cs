using TimeToTwentyfour.scripts.enums.poker;
using Godot;

namespace TimeToTwentyfour.scripts.entities.poker;

/// <summary>
///     扑克牌契约
///     定义一张扑克对象的完整生命周期、视觉属性与空间操作。
///     实现类负责提供统一的牌面渲染、动画与交互能力。
/// </summary>
/// <remarks>
///     <para>
///         由牌桌控制器,牌组控制器,选择控制器持有单张牌的引用：
///     </para>
///     <code>
///         poker.SuitType = SuitType.Heart; // 花色为红桃
///         poker.NumValue = "24"; // 点数为24
///         poker.NumType = NumType.Integer; // 点数类型为整数
///         poker.Shadow = true; // 开启阴影投影
///         poker.Animate = true; // 开启动画
///         poker.Fake3D = true; // 开启伪 3D 缩放
///     </code>
/// </remarks>
public interface IPoker
{
    /// <summary>
    ///    唯一标识符。
    /// </summary>
    /// <returns>唯一标识符 <see cref="Guid"/></returns>
    Guid Id { get; }
    
    /// <summary>
    ///     花色类型。
    /// </summary>
    /// <returns>花色 <see cref="SuitType"/></returns>
    SuitType SuitType { get; set;}
    
    /// <summary>
    ///     点数数值。
    /// </summary>
    /// <returns>点数数值 <see cref="string"/></returns>
    string NumValue { get; set;}
    
    /// <summary>
    ///     点数类型
    /// </summary>
    /// <returns>点数类型 <see cref="NumType"/> </returns>
    NumType NumType { get; set;}
    
    /// <summary>
    ///     是否启用阴影投影。
    ///     开启后牌面底部会渲染一层阴影，增强立体感和层次感。
    /// </summary>
    bool Shadow { get; set; }
    
    /// <summary>
    ///     是否启用动画。
    ///     关闭后所有空间变化（移动、翻转）将瞬间完成，不再播放过渡。
    /// </summary>
    bool Animate { get; set; }
    
    /// <summary>
    ///     单次动画持续时间（秒）。
    ///     影响 <see cref="MoveTo"/>、<see cref="ChangeTo"/>、<see cref="SpawnTo"/> 等过渡时长。
    /// </summary>
    float AnimateTime { get; set; }
    
    /// <summary>
    ///     是否启用伪 3D 缩放（模拟牌面厚度 / 透视）。
    ///     开启后会在旋转或悬浮时附加一个微小的透视变形。
    /// </summary>
    bool Fake3D { get; set; }

    /// <summary>
    ///     是否置顶渲染。
    /// </summary>
    /// <remarks>
    ///     <list type="bullet">
    ///         <item>
    ///             为 <c>true</c> 时，该 <see cref="Godot.CanvasItem"/> 不再继承
    ///             父级 <see cref="Godot.CanvasItem"/> 的变换（位移 / 旋转 / 缩放）。
    ///         </item>
    ///         <item>
    ///             绘制顺序也会发生变化：始终在其他非 TopLevel 的兄弟节点之上渲染。
    ///         </item>
    ///         <item>
    ///             效果等同于把该节点作为裸 <see cref="Godot.Node"/> 的子级，
    ///             常用于拖拽中的牌、弹出动画等需要脱离布局约束的场景。
    ///         </item>
    ///     </list>
    /// </remarks>
    bool TopLevel { get; set; }
    
    /// <summary>
    ///     尺寸
    /// </summary>
    /// <returns>尺寸 <see cref="Vector2"/></returns>
    Vector2 Size { get; }
    
    /// <summary>
    ///     全局坐标
    /// </summary>
    /// <returns>全局坐标 <see cref="Vector2"/></returns>
    Vector2 GlobalPosition { get; set; }
    
    /// <summary>
    ///     旋转弧度
    /// </summary>
    /// <returns>旋转弧度 <see cref="float"/></returns>
    float Rotation { get;}
    
    /// <summary>
    ///     重置位置（"原位"锚点）。
    ///     调用 <c>Reset("position")</c> 时，
    ///     牌将回到此坐标。
    /// </summary>
    Vector2 ResetPosition { get; set; }
    
    /// <summary>
    ///     重置旋转角度（"原位"弧度）。
    ///     调用 <c>Reset("rotation")</c> 时，
    ///     牌将回到此角度。
    /// </summary>
    float ResetRotation { get; set; }
    
    /// <summary>
    ///     获取鼠标的全局位置
    /// </summary>
    /// <returns>鼠标的全局位置 <see cref="Vector2"/></returns>
    Vector2 GetGlobalMousePosition();
    
    /// <summary>
    ///     获取父节点
    /// </summary>
    /// <returns>父节点 <see cref="Node"/></returns>
    Node GetParent();
    
    /// <summary>
    ///     更换父节点
    /// </summary>
    /// <param name="parent">目标节点 <see cref="Node"/></param>
    void Reparent(Node parent);

    /// <summary>
    ///     变更到指定状态
    /// </summary>
    /// <param name="state">目标状态 <see cref="StateType"/></param>
    void ChangeTo(StateType state);

    /// <summary>
    ///     放置到指定位置
    /// </summary>
    /// <param name="position">目标位置 <see cref="Vector2"/></param>
    void SpawnTo(Vector2 position);
    
    /// <summary>
    ///     移动到指定位置
    /// </summary>
    /// <param name="position">目标位置 <see cref="Vector2"/></param>
    void MoveTo(Vector2 position);

    /// <summary>
    ///     重置指定属性
    /// </summary>
    /// <param name="attributeName">
    ///     属性名称，支持：
    ///     <c>"position"</c> → 回到 <see cref="ResetPosition"/>、
    ///     <c>"rotation"</c> → 回到 <see cref="ResetRotation"/>、
    ///     <c>"all"</c> → 全部重置。
    /// </param>
    void Reset(string attributeName);
}