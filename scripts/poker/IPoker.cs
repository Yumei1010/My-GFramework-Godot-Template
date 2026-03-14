using GFrameworkGodotTemplate.scripts.enums.poker;

namespace GFrameworkGodotTemplate.scripts.poker;

/// <summary>
/// 扑克接口，定义了扑克对象的基本属性和必须实现的功能
/// </summary>
public interface IPoker
{
    /// <summary>
    ///     标识符
    /// </summary>
    Guid Id { get; set; }
    
    /// <summary>
    ///     花色类型
    /// </summary>
    SuitType SuitType { get; set; }
    
    /// <summary>
    ///     数值
    /// </summary>
    string NumValue { get; set; }
    
    /// <summary>
    /// 数值类型
    /// </summary>
    NumType NumType { get; set; }
    
    /// <summary>
    /// 标签集合
    /// </summary>
    IList<TagType> Tags { get; set; }
    
    /// <summary>
    /// 状态集合
    /// </summary>
    IList<StateType> States { get; set; }
}