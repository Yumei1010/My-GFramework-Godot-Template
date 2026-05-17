using GFramework.Game.Abstractions.data;
using Godot;

namespace TimeToTwentyfour.scripts.data.annotation_tool;

public sealed class RectElementData : IData
{
    public Vector2 TopLeft { get; set; }
    public Vector2 BottomRight { get; set; }
    public Color Color { get; set; }
    public float Width { get; set; }
}
