namespace TimeToTwentyfour.scripts.entities.poker;

/// <summary>
///     牌桌接口，定义了牌桌对象的基本属性和必须实现的功能。
/// </summary>
public interface IDeck
{
    /// <summary>
    ///     添加扑克
    /// </summary>
    /// <param name="poker">扑克实例 <see cref="IPoker"/></param>
    void Add(IPoker poker);
    
    /// <summary>
    ///     抽出扑克
    /// </summary>
    /// <param name="poker">目标扑克 <see cref="IPoker"/></param>
    /// <returns>目标扑克 <see cref="IPoker"/></returns>
    IPoker Remove(IPoker poker);
}