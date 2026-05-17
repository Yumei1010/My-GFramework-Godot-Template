using Godot;

namespace TimeToTwentyfour.scripts.cqrs.annotation_tool.@event;

public sealed class AnnotationToolCurrentColorChangedEvent
{
    public required Color CurrentColor {get; init; }
}
