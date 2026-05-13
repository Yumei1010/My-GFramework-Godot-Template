using TimeToTwentyfour.scripts.data.timeBar;

namespace CQRS.TimeBar.Query.Result;

public sealed class TimeBarView
{
    public TimeBarData TimeBar {get; set; } = new TimeBarData();
}