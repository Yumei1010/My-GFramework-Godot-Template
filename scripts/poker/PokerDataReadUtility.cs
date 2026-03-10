using GFramework.Core.extensions;
using GFramework.Core.utility;
using GFrameworkGodotTemplate.scripts.data.interfaces;
using GFrameworkGodotTemplate.scripts.serializer;
using Godot;

namespace GFrameworkGodotTemplate.scripts.poker;

/// <summary>
/// 扑克数据读取工具类，负责从存储中读取扑克数据并反序列化
/// </summary>
public class PokerDataReadUtility : AbstractContextUtility, IPokerDataReadUtility
{
    /// <summary>
    /// 扑克数据文件的路径，从项目设置中获取并全局化路径
    /// </summary>
    private static readonly string AsteroidPath = ProjectSettings.GetSetting("application/config/assets/asteroid_path").AsString();
    
    /// <summary>
    /// 扑克数据序列化器，用于将JSON数据反序列化为PokerData对象
    /// </summary>
    private readonly ISerializer<PokerData> _serializer = new PokerDataSerializer();
    
    private Godot.Collections.Dictionary<int, PokerDefinition> _defs = null!;
    
    /// <summary>
    /// 当前读取的扑克数据
    /// </summary>
    public PokerData? CurrentPokerData { get; set; }
    
    /// <summary>
    /// 存储读取工具，用于从文件系统读取数据
    /// </summary>
    private IReadStorageUtility _storage = null!;
    
    /// <summary>
    /// 初始化方法，获取存储读取工具实例
    /// </summary>
    protected override void OnInit()
    {
        _storage = this.GetUtility<IReadStorageUtility>()!;
        Load();
    }
    
    /// <summary>
    /// 从存储中读取扑克数据并反序列化到CurrentPokerData属性中
    /// </summary>
    public void Load()
    {
        var json = _storage.Read(AsteroidPath);
        CurrentPokerData = _serializer.Deserialize(json);
        _defs = new Godot.Collections.Dictionary<int, PokerDefinition>(CurrentPokerData.Definitions.ToDictionary(d => d.Id));
    }
    
    public Godot.Collections.Dictionary<int, PokerDefinition> GetPokerDefinitions()
    {
        return _defs;
    }
}