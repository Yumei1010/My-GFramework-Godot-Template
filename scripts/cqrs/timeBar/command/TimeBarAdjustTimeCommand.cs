using GFramework.Core.command;
using GFramework.Core.extensions;
using TimeToTwentyfour.scripts.cqrs.timeBar.@event;
using TimeToTwentyfour.scripts.model.timeBar;

namespace TimeToTwentyfour.scripts.cqrs.timeBar.command;

public sealed class TimeBarAdjustTimeCommand : AbstractCommand
{
    public float Time {get; init; }

    protected override void OnExecute()
    {
        this.GetModel<TimeBarModel>().TotalDuration += Time;

        this.SendEvent(new TimeBarTimeAdjustedEvent
        {
            Time = Time
        });
    }
}
