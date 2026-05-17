using GFramework.Game.Abstractions.data;
using Godot;
using TimeToTwentyfour.scripts.enums.annotation_tool;

namespace TimeToTwentyfour.scripts.data.annotation_tool;

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
