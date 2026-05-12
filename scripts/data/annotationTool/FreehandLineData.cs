using GFramework.Game.Abstractions.data;
using Godot;

namespace TimeToTwentyfour.scripts.data.annotationTool;

public sealed class FreehandLineData : IData
{
    public List<Vector2> Points { get; set; } = [];
    public Color Color { get; set; }
    public float Width { get; set; }
}
