using GFramework.Core.Abstractions.controller;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using Godot;

namespace TimeToTwentyfour.scripts.entities.timeBar;

[Log]
[ContextAware]
public partial class TimeBar : Control, ITimeBar, IController
{
    private float _remaining;
    private bool _isPaused;

    private bool IsRunning => !Timer.IsStopped();
    private bool IsPaused => _isPaused && !IsRunning;
    private float TimeLeft => IsRunning ? (float)Timer.TimeLeft : (_isPaused ? _remaining : 0f);

    public override void _Ready()
    {
        ConnectSignal();
        Timer.OneShot = true; 
        Start(10);
    }
    
    public void Start(float duration)
    {
        Stop();
        _isPaused = false;
        Timer.WaitTime = duration;
        Timer.Start();
    }
    
    public void Pause()
    {
        if (!IsRunning || _isPaused) return;

        _remaining = (float)Timer.TimeLeft;
        Timer.Stop();
        _isPaused = true;
    }
    
    public void Resume()
    {
        if (!_isPaused) return;

        _isPaused = false;
        Timer.WaitTime = _remaining;
        Timer.Start();
    }
    
    public void Stop()
    {
        Timer.Stop();
        _isPaused = false;
        _remaining = 0f;
    }

    /// <summary>
    /// 增加或减少剩余时间（正数增加，负数减少）
    /// </summary>
    public void AdjustTime(float delta)
    {
        if (IsRunning)
        {
            float newTime = (float)Timer.TimeLeft + delta;
            Timer.Stop();
            _remaining = newTime;
        }
        else if (_isPaused)
        {
            _remaining += delta;
        }
        // 未启动，忽略调整
        else
        {
            return; 
        }

        if (_remaining <= 0f)
        {
            _remaining = 0f;
            Timer.Stop();
            _isPaused = false;
        }
        // 运行中调整后需要重新启动
        else if (!_isPaused)
        {
            Timer.WaitTime = _remaining;
            Timer.Start();
        }
    }
}