using TimeToTwentyfour.scripts.enums.annotation_tool;

namespace TimeToTwentyfour.scripts.component.annotation_tool;

/// <summary>
///     标注工具契约
///     支持直线、圆形、矩形、随手涂鸦绘制与橡皮擦除。
/// </summary>
public partial interface IAnnotationTool
{
    /// <summary>
    ///     是否启用（禁用时不响应输入）
    /// </summary>
    bool Enabled { get; set; }

    /// <summary>
    ///     笔刷半径（像素）
    /// </summary>
    float StrokeWidth { get; set; }

    /// <summary>
    ///     橡皮擦半径（像素）
    /// </summary>
    float EraserRadius { get; set; }

    void ChangeTo(AnnotationToolType tool);
}