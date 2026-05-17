using GFramework.Core.model;
using TimeToTwentyfour.scripts.data.time_bar;

namespace TimeToTwentyfour.scripts.model.time_bar;

public class TimeBarModel : AbstractModel
{
    public float TotalDuration { get; set; } = 120f;

    public float TimeScale { get; set; } = 1f;

    protected override void OnInit()
    {
        
    }

    public T? GetData<T>() where T : class
    {
        if (typeof(T) == typeof(TimeBarData))
        {
            return new TimeBarData
            {
                TotalDuration = TotalDuration,
                TimeScale = TimeScale
            } as T;
        }

        throw new InvalidOperationException($"Unsupported data type: {typeof(T).Name}");
    }
}
