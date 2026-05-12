using Godot;

namespace TimeToTwentyfour.scripts.data.annotationTool;

public sealed class RectElementData
{
    public Vector2 TopLeft { get; set; }
    public Vector2 BottomRight { get; set; }
    public Color Color { get; set; }
    public float Width { get; set; }
}
