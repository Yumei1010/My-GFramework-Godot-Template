using Godot;

namespace TimeToTwentyfour.scripts.component.timeBar;

/// <summary>
///     <see cref="TimeBar"/> 的属性和字段定义文件。
/// </summary>
public partial class TimeBar
{
    public float TotalDuration { get; set; }
    public bool Paused { get; set; }
    public bool IsRunning => Remaining > 0f && !Paused;
    public float TimeScale { get; set { field = value; Mathf.Max(0f, value); } }
    
    private float Remaining { get; set; }
}