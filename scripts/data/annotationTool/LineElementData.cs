using GFramework.Game.Abstractions.data;
using Godot;

namespace TimeToTwentyfour.scripts.data.annotationTool;

public sealed class LineElementData : IData
{
    public Vector2 Start { get; set; }
    public Vector2 End { get; set; }
    public Color Color { get; set; }
    public float Width { get; set; }
}
