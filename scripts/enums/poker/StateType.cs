namespace GFrameworkGodotTemplate.scripts.enums.poker;

/// <summary>
///     定义卡牌状态类型的键值枚举
///     用于标识和管理不同的卡牌状态类型
/// </summary>
public enum StateType
{
    /// <summary>
    ///     手牌堆中
    /// </summary>
    InHand,
    
    /// <summary>
    ///     抽牌堆中
    /// </summary>
    InDrawPile,
    
    /// <summary>
    ///     弃牌堆中
    /// </summary>
    InDiscardPile,
    
    /// <summary>
    ///     对象池中
    /// </summary>
    InPool,
    
    /// <summary>
    ///     拖拽中
    /// </summary>
    OnDrag,
    
    /// <summary>
    ///     展示中
    /// </summary>
    OnDisplay,
    
    /// <summary>
    ///     选择中
    /// </summary>
    OnSelect,
    
    /// <summary>
    ///     未选择
    /// </summary>
    UnSelect
}