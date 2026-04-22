using GFrameworkGodotTemplate.scripts.enums.poker;
using Godot;

namespace GFrameworkGodotTemplate.scripts.poker;

public partial class Poker
{
    [Export] private SuitType SuitType { get; set; } = SuitType.Heart;
    [Export] private string NumValue { get; set; } = "24";
    [Export] private NumType NumType { get; set; } = NumType.Integer;
    
    private Guid Id { get; } = Guid.NewGuid();
    private Vector2 DefaultPosition { get; set; }
    private float DefaultRotation { get; set; }
}