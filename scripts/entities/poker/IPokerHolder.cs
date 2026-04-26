using Godot;

namespace TimeToTwentyfour.scripts.entities.poker;

/// <summary>
///     卡套接口，定义了卡套对象的基本属性和必须实现的功能。
/// </summary>
public interface IPokerHolder
{
     /// <summary>
     ///     是否包含指定扑克
     /// </summary>
     /// <param name="poker">目标扑克 <see cref="IPoker"/></param>
     /// <returns>true/false <see cref="bool"/></returns>
     bool Contains(IPoker poker);

     /// <summary>
     ///     获取内容物
     /// </summary>
     /// <returns>内容物 <see cref="IPoker"/></returns>
     IPoker GetContent();

     /// <summary>
     ///     插入扑克
     /// </summary>
     /// <param name="poker">目标扑克 <see cref="IPoker"/></param>
     void Insert(IPoker poker);
     
     /// <summary>
     ///     抽出扑克
     /// </summary>
     /// <returns>内容物 <see cref="IPoker"/></returns>
     IPoker Extract();

     /// <summary>
     ///     互换内容物
     /// </summary>
     /// <param name="holder">装有目标扑克的卡套 <see cref="IPokerHolder"/></param>
     void Exchange(IPokerHolder holder);

     /// <summary>
     ///     整理内容物位置
     /// </summary>
     void Neaten();
     
     /// <summary>
     ///     清空内容物
     /// </summary>
     void Clear();
     
     /// <summary>
     ///     获取全局坐标
     /// </summary>
     /// <returns>全局坐标 <see cref="Vector2"/></returns>
     Vector2 GetGlobalPosition();
     
     /// <summary>
     ///     放置卡套到指定位置
     /// </summary>
     /// <para>目标坐标 <see cref="Vector2"/></para>>
     void SpawnTo(Vector2 position);
}