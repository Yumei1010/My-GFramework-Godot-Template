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

        this.RegisterEvent<TimeBarStartedEvent>(e =>
        {
            OnStartedEvent();
        }).UnRegisterWhenNodeExitTree(this);

        this.RegisterEvent<TimeBarPausedEvent>(e =>
        {
            OnPausedEvent();
        }).UnRegisterWhenNodeExitTree(this);

        this.RegisterEvent<TimeBarResumedEvent>(e =>
        {
            OnResumedEvent();
        }).UnRegisterWhenNodeExitTree(this);

        this.RegisterEvent<TimeBarStopedEvent>(e =>
        {
            OnStopedEvent();
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
        if (_remaining <= 0f && !Paused) return;

            _remaining += time;

            if (_remaining <= 0f)
            {
                _remaining = 0f;
                _totalDuration = 0f;
                Paused = false;
                UpdateTimeDisplay();
                return;
            }

            if (_remaining > _totalDuration)
                _totalDuration = _remaining;

            UpdateTimeDisplay();
    }

    private void OnStartedEvent()
    {
        if (_totalDuration <= 0f) return;
        Paused = false;
        _remaining = _totalDuration;
    }

    private void OnPausedEvent()
    {
        if (!IsRunning) return;
        Paused = true;
    }

    private void OnResumedEvent()
    {
        if (!Paused) return;
        Paused = false;
    }

    private void OnStopedEvent()
    {
        Paused = false;
        _remaining = 0f;
    }
}
