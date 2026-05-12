using GFramework.Game.Abstractions.data;
using Godot;
using TimeToTwentyfour.scripts.enums.annotationTool;

namespace TimeToTwentyfour.scripts.data.annotationTool;

public class AnnotationToolData : IData
{
    public bool Enabled { get; set; } = false;
    public float ToolWidth { get; set; } = 2.0f;
    public AnnotationToolType CurrentTool { get; set; } = AnnotationToolType.Freehand;
    public Color CurrentColor { get; set; } = new Color(1, 0, 0);

    public void Reset()
    {
        Enabled = false;
        ToolWidth = 2.0f;
        CurrentTool = AnnotationToolType.Freehand;
        CurrentColor = new Color(1, 0, 0);
    }
}
