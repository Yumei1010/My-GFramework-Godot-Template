namespace TimeToTwentyfour.scripts.entities.poker;

/// <summary>
///     扑克工厂接口，定义了扑克工厂的基本属性和必须实现的功能。
/// </summary>
public interface IPokerFactory
{
    /// <summary>
    ///     生产
    /// </summary>
    /// <returns>产品 <see cref="IPoker"/></returns>
    IPoker Product();
}