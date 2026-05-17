namespace TimeToTwentyfour.scripts.cqrs.annotation_tool.@event;

public sealed class AnnotationToolEnableChangedEvent
{
    public required bool Enabled {get; init; }
}
