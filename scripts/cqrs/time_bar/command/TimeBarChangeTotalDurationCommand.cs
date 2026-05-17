using GFramework.Core.command;
using GFramework.Core.extensions;
using TimeToTwentyfour.scripts.cqrs.time_bar.@event;
using TimeToTwentyfour.scripts.model.time_bar;

namespace TimeToTwentyfour.scripts.cqrs.time_bar.command;

public sealed class TimeBarChangeTotalDurationCommand : AbstractCommand
{
    public required float TotalDuration { get; set; }

    protected override void OnExecute()
    {
        this.GetModel<TimeBarModel>().TotalDuration = TotalDuration;

        this.SendEvent(new TimeBarTotalDurationChangedEvent 
        {
             TotalDuration = TotalDuration 
        });
    }
}
