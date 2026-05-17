using TimeToTwentyfour.scripts.data.time_bar;

namespace CQRS.TimeBar.Query.Result;

public sealed class TimeBarView
{
    public TimeBarData TimeBar {get; set; } = new TimeBarData();
}