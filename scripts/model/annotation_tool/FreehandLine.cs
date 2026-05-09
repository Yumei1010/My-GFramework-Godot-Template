using Godot;

namespace TimeToTwentyfour.scripts.model.annotation_tool;
public sealed class FreehandLine
{
    public List<Vector2> Points { get; set; } = [];
    public Color Color { get; set; }
    public float Width { get; set; }
}