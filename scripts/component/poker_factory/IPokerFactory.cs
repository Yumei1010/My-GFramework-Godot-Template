using TimeToTwentyfour.scripts.entities.poker;
using TimeToTwentyfour.scripts.enums.poker;
using TimeToTwentyfour.scripts.model.pileModel;

namespace TimeToTwentyfour.scripts.component.pokerFactory;

/// <summary>
///     扑克工厂接口，定义了扑克工厂的基本属性和必须实现的功能。
/// </summary>
public interface IPokerFactory
{
    /// <summary>
    ///     创建一个空白扑克实体实例。
    /// </summary>
    /// <returns>产品 <see cref="IPoker"/></returns>
    IPoker Product();

    /// <summary>
    ///     创建扑克实体实例并直接设置花色、数值与数值类型。
    /// </summary>
    /// <param name="suitType">花色 <see cref="SuitType"/></param>
    /// <param name="numValue">点数数值</param>
    /// <param name="numType">数值类型，默认 <see cref="NumType.Integer"/></param>
    /// <returns>已配置的扑克实例 <see cref="IPoker"/></returns>
    IPoker Product(SuitType suitType, string numValue, NumType numType = NumType.Integer);

    /// <summary>
    ///     从 <see cref="Card"/> 数据创建扑克实体实例，自动关联 Id。
    /// </summary>
    /// <param name="card">牌面数据 <see cref="Card"/></param>
    /// <returns>已配置且 Id 关联的扑克实例 <see cref="IPoker"/></returns>
    IPoker Product(Card card);
}