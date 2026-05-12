using Godot;

namespace TimeToTwentyfour.scripts.cqrs.annotationTool.@event;

public sealed class AnnotationToolCurrentColorChangedEvent
{
    public Color CurrentColor {get; init; }
}
