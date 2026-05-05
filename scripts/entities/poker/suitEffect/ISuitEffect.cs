using TimeToTwentyfour.scripts.enums.poker;

namespace TimeToTwentyfour.scripts.entities.poker.suitEffect;

public interface ISuitEffect
{
    /// <summary>
    ///     本花色特效对应的花色特效类型枚举。
    ///     每个 <see cref="ISuitEffect"/> 实现类对应唯一一个 <see cref="EffectType"/>。
    /// </summary>
    EffectType EffectType { get; set; }
    
    /// <summary>
    ///     抽牌回调。
    /// </summary>
    void Drawn();
    
    /// <summary>
    ///     出牌回调。
    /// </summary>
    void Checked();
    
    /// <summary>
    ///     弃牌回调。
    /// </summary>
    void Folded();
    
    /// <summary>
    ///     计算开始回调。
    /// </summary>
    void CalculationStarted();
    
    /// <summary>
    ///     计算结束回调。
    /// </summary>
    void CalculationFinish();
}