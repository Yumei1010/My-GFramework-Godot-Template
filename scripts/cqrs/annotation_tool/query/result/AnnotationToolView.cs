using TimeToTwentyfour.scripts.data.annotation_tool;

namespace TimeToTwentyfour.scripts.cqrs.annotation_tool.query.result;

public sealed class AnnotationToolView
{
    public AnnotationToolData AnnotationTool { get; set; } = new AnnotationToolData();
}