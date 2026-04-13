using GFrameworkGodotTemplate.scripts.poker;

namespace GFrameworkGodotTemplate.scripts.events.pokerSelector;

/// <summary>
/// 扑克选择器容量变更事件类
/// 用于表示扑克选择器容量发生变化的事件
/// </summary>
public class CapacityChangedEvent
{
    /// <summary>
    /// 是否满载
    /// </summary>
    public bool IsFull { get; set; }
    
    /// <summary>
    /// 载荷
    /// </summary>
    public IList<IPoker> Loads { get; set; } = new List<IPoker>();
}