using Godot;

namespace TimeToTwentyfour.scripts.cqrs.annotation_tool.@event;

public sealed class AnnotationToolCurrentColorChangedEvent
{
    public Color CurrentColor {get; init; }
}
