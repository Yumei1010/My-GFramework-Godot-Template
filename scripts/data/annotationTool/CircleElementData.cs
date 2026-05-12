using GFramework.Game.Abstractions.data;
using Godot;

namespace TimeToTwentyfour.scripts.data.annotationTool;

public sealed class CircleElementData : IData
{
    public Vector2 Center { get; set; }
    public float Radius { get; set; }
    public Color Color { get; set; }
    public float Width { get; set; }
}
