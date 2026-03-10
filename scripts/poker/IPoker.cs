namespace GFrameworkGodotTemplate.scripts.poker;

/// <summary>
/// 扑克接口，定义了扑克对象必须实现的基本功能
/// </summary>
public interface IPoker
{
    /// <summary>
    /// 初始化扑克对象
    /// </summary>
    /// <param name="definition">扑克定义对象，包含扑克的配置信息</param>
    void Init(PokerDefinition definition);
}