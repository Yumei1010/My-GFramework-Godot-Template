using TimeToTwentyfour.scripts.data.annotationTool;

namespace cqrs.poker.query.result;

public sealed class AnnotationToolView
{
    public AnnotationToolData AnnotationTool { get; set; } = new AnnotationToolData();
}