using GFramework.Core.Abstractions.system;
using GFrameworkGodotTemplate.scripts.poker;

namespace GFrameworkGodotTemplate.scripts.pile;

/// <summary>
///     牌堆系统接口，用于管理扑克
/// </summary>
public interface IPileSystem : ISystem
{
    /// <summary>
    ///     从牌堆中获取一个可用的扑克对象,具体实现由子类完成
    /// </summary>
    /// <returns>返回获取到的Poker对象实例</returns>
    Poker Draw();
    
    /// <summary>
    ///     从扑克对象池中获取一个指定的扑克对象,具体实现由子类完成
    /// </summary>
    /// <param name="key">场景键值，用于标识特定的扑克</param>
    /// <returns>返回获取到的Poker对象实例</returns>
    Poker Get(string key);
    
    /// <summary>
    ///     往牌堆中放入一个指定的扑克对象,具体实现由子类完成
    /// </summary>
    /// <param name="poker">特定的Poker对象实例</param>
    void Put(Poker poker);
    
    /// <summary>
    ///     往牌堆中放入一个指定的扑克对象,具体实现由子类完成
    /// </summary>
    /// <param name="poker">特定的Poker对象实例</param>
    void Add(Poker poker);
    
    /// <summary>
    ///     从牌堆中移除一个指定的扑克对象,具体实现由子类完成
    /// </summary>
    /// <param name="key">场景键值，用于标识特定的扑克</param>
    void Remove(string key);
    
    /// <summary>
    ///     洗牌方法,具体实现由子类完成
    /// </summary>
    void Shuffle();
    
    /// <summary>
    ///     清空牌堆方法,具体实现由子类完成
    /// </summary>
    void Clear();
}