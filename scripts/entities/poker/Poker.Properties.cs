using TimeToTwentyfour.scripts.enums.poker;
using Godot;

namespace TimeToTwentyfour.scripts.entities.poker;

public partial class Poker
{
    /// <summary>
    ///     花色类型 <see cref="SuitType"/>
    /// </summary>
    [Export] private SuitType SuitType { get; set; } = SuitType.Heart;
    
    /// <summary>
    ///     点数数值 <see cref="String"/>
    /// </summary>
    [Export] private string NumValue { get; set; } = "24";
    
    /// <summary>
    ///     数值类型 <see cref="NumType"/>
    /// </summary>
    [Export] private NumType NumType { get; set; } = NumType.Integer;
    
    /// <summary>
    ///     唯一标识符 <see cref="Guid"/>
    /// </summary>
    private Guid Id { get; } = Guid.NewGuid();
    
    /// <summary>
    ///     默认坐标 <see cref="Vector2"/>
    /// </summary>
    [Export] private Vector2 DefaultPosition { get; set; }
    
    /// <summary>
    ///     默认偏转角度 <see cref="float"/>
    /// </summary>
    [Export] private float DefaultRotation { get; set; }
}