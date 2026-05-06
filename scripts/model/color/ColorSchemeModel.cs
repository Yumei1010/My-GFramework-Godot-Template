using GFramework.Core.model;

namespace TimeToTwentyfour.scripts.model.color;

/// <summary>
///     色彩方案模型，持有当前时间阶段色值，供查询与阶段切换。
/// </summary>
public class ColorSchemeModel : AbstractModel
{
    private TimePhaseColor _currentPhase = TimePhaseColorConfig.Resolve(0);

    /// <summary>当前时间阶段色值。</summary>
    public TimePhaseColor CurrentPhase => _currentPhase;

    protected override void OnInit()
    {
    
    }

    /// <summary>根据小时值更新当前阶段。若阶段变化则返回新的 <see cref="TimePhaseColor"/>，否则返回 null!。</summary>
    /// <remarks>调用方应在返回非 null 时发送 <see cref="scripts.cqrs.color.@event.ColorSchemeChangedEvent"/>。</remarks>
    public TimePhaseColor UpdateHour(int hour)
    {
        var newPhase = TimePhaseColorConfig.Resolve(hour);
        if (newPhase == _currentPhase)
            return null!;

        _currentPhase = newPhase;
        return newPhase;
    }
}
