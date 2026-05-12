using TimeToTwentyfour.scripts.enums.annotationTool;

namespace TimeToTwentyfour.scripts.component.annotationTool;

/// <summary>
///     标注工具契约
///     支持直线、圆形、矩形、随手涂鸦绘制与橡皮擦除。
/// </summary>
public partial interface IAnnotationTool
{
    
    /// <summary>
    ///     切换当前绘制工具
    /// </summary> 
    /// <param name="tool">要切换到的工具类型 <see cref="AnnotationToolType"/></param>
    void ChangeTo(AnnotationToolType tool);
}