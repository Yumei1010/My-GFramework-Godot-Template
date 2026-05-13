using GFramework.Core.extensions;
using GFramework.Godot.extensions;
using TimeToTwentyfour.scripts.cqrs.timeBar.@event;

namespace TimeToTwentyfour.scripts.component.timeBar;

public partial class TimeBar
{
    private void RegisterEvent()
    {
        this.RegisterEvent<TimeBarTotalDurationChangedEvent>(e =>
        {
            OnTotalDurationChangedEvent(e.TotalDuration);
        }).UnRegisterWhenNodeExitTree(this);

        this.RegisterEvent<TimeBarTimeScaleChangedEvent>(e =>
        {
            OnTimeScaleChangedEvent(e.TimeScale);
        }).UnRegisterWhenNodeExitTree(this);

        this.RegisterEvent<TimeBarTimeAdjustedEvent>(e =>
        {
            OnTimeAdjustedEvent(e.Time);
        }).UnRegisterWhenNodeExitTree(this);
    }

    private void OnTotalDurationChangedEvent(float totalDuration)
    {
        _totalDuration = totalDuration;
    }

    private void OnTimeScaleChangedEvent(float timeScale)
    {
        _timeScale = timeScale;
    }

    private void OnTimeAdjustedEvent(float time)
    {
        if (Remaining <= 0f && !Paused) return;

            Remaining += time;

            if (Remaining <= 0f)
            {
                Remaining = 0f;
                _totalDuration = 0f;
                Paused = false;
                UpdateTimeDisplay();
                return;
            }

            if (Remaining > _totalDuration)
                _totalDuration = Remaining;

            UpdateTimeDisplay();
    }
}
