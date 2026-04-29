using TimeToTwentyfour.scripts.enums.poker;
using Godot;

namespace TimeToTwentyfour.scripts.entities.poker;

public partial class Poker
{
    [ExportGroup("Style")]
    [ExportSubgroup("Suit")]
    [Export] public SuitType SuitType { get; set; } = SuitType.Heart;
    
    [ExportSubgroup("Value")]
    [Export] public string NumValue { get; set; } = "24";
    [Export] public NumType NumType { get; set; } = NumType.Integer;
        
    [ExportGroup("Animation")]
    [Export] public bool Shadow { get; set; } = true;
    [Export] public bool Animate { get; set; } = true;
    [Export] public float AnimateTime { get; set; } = 0.25f;
    [Export] public bool Fake3D { get; set; } = true;
    
    [ExportGroup("Debug")]
    [Export] public Vector2 ResetPosition { get; set; } = new (0, 0);

    [Export] public float ResetRotation { get; set; }
    
    public Guid Id { get; set; } = Guid.NewGuid();
}