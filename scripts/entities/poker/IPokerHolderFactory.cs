namespace TimeToTwentyfour.scripts.entities.poker;

/// <summary>
///     扑克卡套工厂接口，定义了扑克卡套工厂的基本属性和必须实现的功能。
/// </summary>
public interface IPokerHolderFactory
{
    /// <summary>
    ///     生产
    /// </summary>
    /// <returns>产品 <see cref="IPokerHolder"/></returns>
    IPokerHolder Product();
}