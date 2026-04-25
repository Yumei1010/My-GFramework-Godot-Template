namespace TimeToTwentyfour.scripts.entities.poker;

/// <summary>
///     牌桌接口，定义了牌桌对象的基本属性和必须实现的功能。
/// </summary>
public interface IDeck
{
    void Add(IPokerHolder pokerHolder);
}