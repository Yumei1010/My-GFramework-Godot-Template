using GFramework.Core.Abstractions.system;
using Godot;

namespace GFrameworkGodotTemplate.scripts.poker;

/// <summary>
///     扑克对象池系统接口，用于管理扑克对象的获取和回收
/// </summary>
public interface IPokerPoolSystem : ISystem
{
    /// <summary>
    ///     从扑克对象池中获取一个可用的扑克对象
    /// </summary>
    /// <param name="key">场景键值，用于标识特定的扑克</param>
    /// <param name="parent">父节点，指定扑克对象的父级容器</param>
    /// <returns>返回获取到的Poker对象实例</returns>
    Poker Acquire(string key, Node parent);
    
    /// <summary>
    ///     将使用完毕的扑克对象归还到对象池中
    /// </summary>
    /// <param name="key">场景键值，用于标识特定的扑克</param>
    /// <param name="poker">需要回收的Poker对象实例</param>
    void Release(string key, Poker poker);
}