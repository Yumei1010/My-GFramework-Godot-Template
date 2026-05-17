using GFramework.Core.Abstractions.controller;
using GFramework.Core.extensions;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using Godot;
using TimeToTwentyfour.scripts.cqrs.timeBar.command;
using TimeToTwentyfour.scripts.cqrs.timeBar.@event;

namespace TimeToTwentyfour.scripts.entities.timeBar;

/// <summary>
///     时间轴契约实现类
/// </summary>
[Log]
[ContextAware]
public partial class TimeBar : Control, ITimeBar, IController
{
    public override void _Ready()
    {
        _ = ReadyAsync();
        RegisterEvent();
    }
    
    public override void _Process(double delta)
    {
        // 如果暂停中，返回，否则，继续计时
        if (!IsRunning) return;

        // 按时间倍率扣减
        _remaining -= (float)delta * TimeScale;

        // 更新时间显示
        UpdateTimeDisplay();

        // 如果倒计时未归零，返回，否则，结束记时
        if (!(_remaining <= 0f)) return;
        _remaining = 0f;
        _totalDuration = 0f;
        Paused = false;
        
        this.SendEvent(new TimeBarTimeoutedEvent());
    }

    public void Start(float duration)
    {
        _totalDuration = duration;
        this.SendCommand(new TimeBarStartCommand());

        UpdateTimeDisplay();
    }

    public void Start()
    {
        if (_totalDuration <= 0f) return;
        this.SendCommand(new TimeBarStartCommand());
        
        UpdateTimeDisplay();
    }

    public void Pause()
    {
        this.SendCommand(new TimeBarPauseCommand());
    }

    public void Resume()
    {
        this.SendCommand(new TimeBarResumeCommand());
    }

    public void Stop()
    {
        this.SendCommand(new TimeBarStopCommand());

        UpdateTimeDisplay();
    }
    
    public void AdjustTime(float delta)
    {
        this.SendCommand(new TimeBarAdjustTimeCommand { Time = delta });
    }
    
    private void UpdateTimeDisplay()
    {
        TimeLabel.Text = FormatTime(_remaining);

        if (_totalDuration > 0f)
        {
            TimeProgressBar.Value = _remaining / _totalDuration * 100f;
        }
        else
        {
            TimeProgressBar.Value = 0f;
        }
    }
    
    internal static string FormatTime(float seconds)
    {
        if (seconds <= 0f) return "00:00";

        int totalSeconds = Mathf.CeilToInt(seconds);
        int mins = totalSeconds / 60;
        int secs = totalSeconds % 60;
        return $"{mins:D2}:{secs:D2}";
    }
}