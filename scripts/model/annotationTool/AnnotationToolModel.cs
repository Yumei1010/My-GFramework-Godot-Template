using GFramework.Core.model;
using Godot;
using TimeToTwentyfour.scripts.enums.annotationTool;
using TimeToTwentyfour.scripts.data.annotationTool;

namespace TimeToTwentyfour.scripts.model.annotationTool;

public class AnnotationToolModel : AbstractModel
{
    public bool Enabled { get; set; } = false;
    public float ToolWidth { get; set; } = 2.0f;
    public AnnotationToolType CurrentTool { get; set; } = AnnotationToolType.Freehand;
    public Color CurrentColor { get; set; } = Colors.Red;

    protected override void OnInit()
    {
        
    }

    public T? GetData<T>() where T : class
    {
        if (typeof(T) == typeof(AnnotationToolData))
        {
            return new AnnotationToolData
            {
                Enabled = Enabled,
                ToolWidth = ToolWidth,
                CurrentTool = CurrentTool,
                CurrentColor = CurrentColor
            } as T;
        }

        throw new InvalidOperationException($"Unsupported data type: {typeof(T).Name}");
    }
}
