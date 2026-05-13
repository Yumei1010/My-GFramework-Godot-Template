using TimeToTwentyfour.scripts.enums.annotationTool;
using Godot;

namespace TimeToTwentyfour.scripts.component.annotationTool;

/// <summary>
///     标注工具契约
///     支持直线、圆形、矩形、随手涂鸦绘制与橡皮擦除。
/// </summary>
public partial interface IAnnotationTool
{
    Color Color { get; }

    AnnotationToolType CurrentTool { get; }

    bool Enabled { get; }

    float ToolWidth { get; }
    
    void ChangeTo(AnnotationToolType tool);
}