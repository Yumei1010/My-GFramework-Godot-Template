using Godot;

namespace TimeToTwentyfour.scripts.entities.poker;

/// <summary>
///     卡套接口，定义了卡套对象的基本属性和必须实现的功能。
/// </summary>
public interface IPokerHolder
{
     /// <summary>
     ///  获取全局坐标
     /// </summary>
     /// <returns>全局坐标 <see cref="Vector2"/></returns>
     Vector2 GetGlobalPosition();
     
     /// <summary>
     ///  放置卡套到指定位置
     /// </summary>
     /// <para>目标坐标 <see cref="Vector2"/></para>>
     void SpawnTo(Vector2 position);
}