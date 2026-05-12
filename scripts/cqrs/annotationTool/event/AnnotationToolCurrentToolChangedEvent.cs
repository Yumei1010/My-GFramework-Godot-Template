using TimeToTwentyfour.scripts.enums.annotationTool;

namespace TimeToTwentyfour.scripts.cqrs.annotationTool.@event;

public sealed class AnnotationToolCurrentToolChangedEvent
{
    public AnnotationToolType CurrentTool { get; init; }
}
