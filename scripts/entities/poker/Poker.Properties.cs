using TimeToTwentyfour.scripts.enums.poker;
using Godot;

namespace TimeToTwentyfour.scripts.entities.poker;

public partial class Poker
{
    [ExportGroup("Style")]
        [ExportSubgroup("Suit")]
            [Export] private SuitType SuitType { get; set; } = SuitType.Heart;
            [Export] private Texture2D SuitTexture { get; set; } = GD.Load<Texture2D>("res://assets/textures/poker/surface/heart.png");
    
        [ExportSubgroup("Value")]
            [Export] private string NumValue { get; set; } = "24";
            [Export] private NumType NumType { get; set; } = NumType.Integer;
        
    [ExportGroup("Animation")]
        [Export] private bool Shadow { get; set; } = true;
        [Export] private bool TweenAnimate { get; set; } = true;
        [Export] private float TweenAnimateTime { get; set; } = 0.25f;
        [Export] private Vector2 HoverScaleRate { get; set; } = new (1.2f,1.2f);
        [Export] private bool Fake3D { get; set; } = true;
    
     private Guid Id { get; } = Guid.NewGuid();
}