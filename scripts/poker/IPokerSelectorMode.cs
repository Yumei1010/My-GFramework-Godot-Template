using GFrameworkGodotTemplate.scripts.enums.calculate;

namespace GFrameworkGodotTemplate.scripts.poker;

/// <summary>
///     扑克选择器模式接口，定义了扑克选择器模式的基本属性和必须实现的功能
/// </summary>
public interface IPokerSelectorMode
{
    /// <summary>
    ///     计算时调用的方法
    /// </summary>
    void Calculate();

    /// <summary>
    ///     设置参与计算的扑克实例列表
    /// </summary>
    /// <param name="pokers">参与计算的扑克实例列表 <see cref="IPoker"/></param>
    void SetPokers(IList<IPoker> pokers);
    
    /// <summary>
    ///     获取模式标识符
    /// </summary>
    /// <returns>模式标识符 <see cref="ModeType"/></returns>
    ModeType GetModeType();
    
    /// <summary>
    ///     获取该模式计算所需的扑克数量
    /// </summary>
    /// <returns>该模式计算所需的扑克数量 <see cref="int"/></returns>
    int GetCapacity();

    /// <summary>
    ///     获取该模式参与计算的扑克实例列表
    /// </summary>
    /// <returns>该模式参与计算的扑克实例列表 <see cref="IList{IPoker}"/></returns>
    IList<IPoker> GetPokers();
}