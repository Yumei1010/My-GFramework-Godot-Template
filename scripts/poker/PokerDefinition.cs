using Godot;

namespace GFrameworkGodotTemplate.scripts.poker;

/// <summary>
/// 扑克定义类，用于描述扑克的基本属性和配置信息
/// </summary>
public partial class PokerDefinition : Node
{
    /// <summary>
    /// 扑克的唯一标识符
    /// </summary>
    public int Id { get; private set; }
    
    public required string BaseValue { get; set; }
    
    public required string BaseSuit { get; set; }
    
    public required string SceneKey { get; set; }
}