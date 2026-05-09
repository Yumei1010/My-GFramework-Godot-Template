using TimeToTwentyfour.scripts.enums.annotation_tool;
using GFramework.Core.model;

namespace TimeToTwentyfour.scripts.model.annotation_tool;

public class AnnotationToolModel : AbstractModel
{
    public bool Enabled { get; set; }
    public float ToolWidth { get; set; } = 2.0f;
    public AnnotationToolType CurrentTool { get; set; }

    protected override void OnInit()
    {

    }
}
