using GFramework.Game.Abstractions.data;
using Godot;
using TimeToTwentyfour.scripts.enums.annotationTool;

namespace TimeToTwentyfour.scripts.data.annotationTool;

public class AnnotationToolData : IData
{
    public bool Enabled { get; set; }
    public float ToolWidth { get; set; }
    public AnnotationToolType CurrentTool { get; set; }
    public Color CurrentColor { get; set; }

    public void Reset()
    {
        Enabled = false;
        ToolWidth = 2.0f;
        CurrentTool = AnnotationToolType.Freehand;
        CurrentColor = new Color(1, 0, 0);
    }
}
