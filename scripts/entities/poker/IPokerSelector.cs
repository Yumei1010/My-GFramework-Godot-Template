using GFrameworkGodotTemplate.scripts.enums.poker;

namespace GFrameworkGodotTemplate.scripts.entities.poker;

/// <summary>
///     扑克选择器接口，定义了扑克选择器的基本属性和必须实现的功能
/// </summary>
public interface IPokerSelector
{
    /// <summary>
    ///     初始化扑克选择器
    /// </summary>
    void Init();

    /// <summary>
    ///     计算
    /// </summary>
    void Calculate();

    /// <summary>
    ///     变更到指定模式
    /// </summary>
    /// <param name="mode">目标状态 <see cref="ModeType"/> </param>
    void ChangeTo(ModeType mode);

    /// <summary>
    ///     清空选择对象
    /// </summary>
    void Clear();
}