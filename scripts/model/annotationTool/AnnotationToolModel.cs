using TimeToTwentyfour.scripts.enums.annotationTool;
using GFramework.Core.model;

namespace TimeToTwentyfour.scripts.model.annotationTool;

public class AnnotationToolModel : AbstractModel
{
    public bool Enabled { get; set; }
    public float ToolWidth { get; set; } = 2.0f;
    public AnnotationToolType CurrentTool { get; set; }

    protected override void OnInit()
    {
        
    }
}
