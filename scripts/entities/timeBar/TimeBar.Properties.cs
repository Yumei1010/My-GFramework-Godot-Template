using Godot;

namespace TimeToTwentyfour.scripts.entities.timeBar;

public partial class TimeBar
{
    public float TotalDuration { get; set; }
    public bool Paused { get; set; }
    public bool IsRunning => Remaining > 0f && !Paused;
    public float TimeScale { get; set { field = value; Mathf.Max(0f, value); } }
    
    private float Remaining { get; set; }
}