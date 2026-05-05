using TimeToTwentyfour.scripts.enums.poker;

namespace TimeToTwentyfour.scripts.entities.poker.suitEffect;

public abstract class SuitEffect : ISuitEffect
{
    /// <summary>
    ///     本特效对应的 <see cref="EffectType"/> 枚举值。
    /// </summary>
    public EffectType EffectType { get; set; }
    
    /// <inheritdoc />
    public virtual void Drawn() { }

    /// <inheritdoc />
    public virtual void Checked() { }

    /// <inheritdoc />
    public virtual void Folded() { }

    /// <inheritdoc />
    public virtual void CalculationStarted() { }

    /// <inheritdoc />
    public virtual void CalculationFinish() { }
}