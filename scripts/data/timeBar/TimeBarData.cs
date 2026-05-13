using GFramework.Game.Abstractions.data;

namespace TimeToTwentyfour.scripts.data.timeBar;

public class TimeBarData : IData
{
    public float TotalDuration { get; set; }

    public float TimeScale { get; set; }

    public void Reset()
    {
        TotalDuration = 120f;
        TimeScale = 1f;
    }
}
