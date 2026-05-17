using TimeToTwentyfour.scripts.enums.annotation_tool;

namespace TimeToTwentyfour.scripts.cqrs.annotation_tool.@event;

public sealed class AnnotationToolCurrentToolChangedEvent
{
    public AnnotationToolType CurrentTool { get; init; }
}
