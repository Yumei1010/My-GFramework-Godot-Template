using TimeToTwentyfour.scripts.enums.annotation_tool;
using Godot;

namespace TimeToTwentyfour.scripts.entities.annotation_tool;

/// <summary>
///     标注工具契约
///     支持直线、圆形、矩形、随手涂鸦绘制与橡皮擦除。
/// </summary>
public interface IAnnotationTool
{
    Color Color { get; }

    AnnotationToolType CurrentTool { get; }

    bool Enabled { get; }

    float ToolWidth { get; }
    
    void ChangeTo(AnnotationToolType tool);
}