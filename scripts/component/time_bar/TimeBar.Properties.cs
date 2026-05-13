namespace TimeToTwentyfour.scripts.component.timeBar;

/// <summary>
///     <see cref="TimeBar"/> 的属性和字段定义文件。
/// </summary>
public partial class TimeBar
{
    public float TotalDuration => _totalDuration;
    public bool Paused { get; set; }
    public bool IsRunning => Remaining > 0f && !Paused;
    public float TimeScale => _timeScale;
    
    private float _totalDuration;
    private float _timeScale;

    private float Remaining { get; set; }
}