namespace GFrameworkGodotTemplate.scripts.poker;

/// <summary>
/// 扑克数据类，用于存储扑克相关数据集合
/// </summary>
public class PokerData
{
    /// <summary>
    /// 扑克定义列表，存储所有扑克的定义信息
    /// </summary>
    public List<PokerDefinition> Definitions { get; set; } = null!;
}