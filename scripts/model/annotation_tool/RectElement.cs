using Godot;

namespace TimeToTwentyfour.scripts.model.annotation_tool;

public sealed class RectElement
{
    public Vector2 TopLeft { get; set; }
    public Vector2 BottomRight { get; set; }
    public Color Color { get; set; }
    public float Width { get; set; }
}