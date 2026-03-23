using GFrameworkGodotTemplate.scripts.poker;

namespace GFrameworkGodotTemplate.scripts.events.Calculate;

/// <summary>
/// 计算事件类
/// 用于表示计算的事件
/// </summary>
public abstract class CalculatedEvent
{
    /// <summary>
    /// 计算方式
    /// </summary>
    public required string Operate { get; set; } = null!;
    
    /// <summary>
    /// 响应事件的poker实例
    /// </summary>
    public IList<Poker> Pokers { get; set; } = null!;
}