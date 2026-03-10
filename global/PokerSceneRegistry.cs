using GFrameworkGodotTemplate.scripts.poker;
using Godot;

namespace GFrameworkGodotTemplate.global;

/// <summary>
/// 扑克场景注册表，继承自Node，实现IPokerSceneRegistry接口，管理扑克场景资源
/// </summary>
public partial class PokerSceneRegistry : Node, IPokerSceneRegistry
{
    /// <summary>
    /// 导出的场景字典，存储扑克场景键值对
    /// </summary>
    [Export] public Godot.Collections.Dictionary<string, PackedScene> Scenes { get; set; } = null!;
    
    public static PokerSceneRegistry Instance { get;private set; } = null!;

    public override void _Ready()
    {
        Instance = this;
    }

    /// <summary>
    /// 根据指定的键获取打包的场景
    /// </summary>
    /// <param name="key">用于标识场景的键值</param>
    /// <returns>与键关联的打包场景对象</returns>
    public PackedScene Get(string key)
    {
        return Scenes[key];
    }
}