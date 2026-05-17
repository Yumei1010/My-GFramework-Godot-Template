using TimeToTwentyfour.scripts.data.annotation_tool;

namespace cqrs.poker.query.result;

public sealed class AnnotationToolView
{
    public AnnotationToolData AnnotationTool { get; set; } = new AnnotationToolData();
}