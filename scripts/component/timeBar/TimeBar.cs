using GFramework.Core.Abstractions.controller;
using GFramework.Core.extensions;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using Godot;
using TimeToTwentyfour.scripts.cqrs.timeBar.@event;

namespace TimeToTwentyfour.scripts.component.timeBar;

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
    }
    
    public override void _Process(double delta)
    {
        // 如果暂停中，返回，否则，继续计时
        if (!IsRunning) return;

        // 按时间倍率扣减
        Remaining -= (float)delta * TimeScale;

        // 更新时间显示
        UpdateTimeDisplay();

        // 如果倒计时未归零，返回，否则，结束记时
        if (!(Remaining <= 0f)) return;
        Remaining = 0f;
        TotalDuration = 0f;
        Paused = false;
        
        ContextAwareExtensions.SendEvent(this, new TimeBarTimeoutedEvent());
    }

    /// <summary>启动指定秒数的倒计时。</summary>
    public void Start(float duration)
    {
        Stop();
        TotalDuration = duration;
        Remaining = duration;
        Paused = false;
        UpdateTimeDisplay();
    }

    /// <summary>暂停倒计时。</summary>
    public void Pause()
    {
        if (!IsRunning) return;
        Paused = true;
    }

    /// <summary>恢复倒计时。</summary>
    public void Resume()
    {
        if (!Paused) return;
        Paused = false;
    }

    /// <summary>停止倒计时并归零。</summary>
    public void Stop()
    {
        Paused = false;
        Remaining = 0f;
        TotalDuration = 0f;
        
        UpdateTimeDisplay();
    }
    
    /// <summary>调整剩余时间（正值增加，负值减少）。</summary>
    public void AdjustTime(float delta)
    {
        if (Remaining <= 0f && !Paused) return;

        Remaining += delta;

        if (Remaining <= 0f)
        {
            Remaining = 0f;
            TotalDuration = 0f;
            Paused = false;
            UpdateTimeDisplay();
            return;
        }
        
        if (Remaining > TotalDuration)
        {
            TotalDuration = Remaining;
        }

        UpdateTimeDisplay();
    }
    
    private void UpdateTimeDisplay()
    {
        // 更新文本
        TimeLabel.Text = FormatTime(Remaining);

        // 更新进度条
        if (TotalDuration > 0f)
        {
            TimeProgressBar.Value = Remaining / TotalDuration * 100f;
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