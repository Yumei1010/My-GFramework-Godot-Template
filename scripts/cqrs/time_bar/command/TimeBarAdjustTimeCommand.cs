using GFramework.Core.command;
using GFramework.Core.extensions;
using TimeToTwentyfour.scripts.cqrs.time_bar.@event;
using TimeToTwentyfour.scripts.model.time_bar;

namespace TimeToTwentyfour.scripts.cqrs.time_bar.command;

public sealed class TimeBarAdjustTimeCommand : AbstractCommand
{
    public required float Time {get; set; }

    protected override void OnExecute()
    {
        this.GetModel<TimeBarModel>().TotalDuration += Time;

        this.SendEvent(new TimeBarTimeAdjustedEvent
        {
            Time = Time
        });
    }
}
