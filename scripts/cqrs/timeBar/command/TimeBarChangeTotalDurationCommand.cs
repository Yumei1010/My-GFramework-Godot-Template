using GFramework.Core.command;
using GFramework.Core.extensions;
using TimeToTwentyfour.scripts.cqrs.timeBar.@event;
using TimeToTwentyfour.scripts.model.timeBar;

namespace TimeToTwentyfour.scripts.cqrs.timeBar.command;

public sealed class TimeBarChangeTotalDurationCommand : AbstractCommand
{
    public float TotalDuration { get; init; }

    protected override void OnExecute()
    {
        this.GetModel<TimeBarModel>().TotalDuration = TotalDuration;

        this.SendEvent(new TimeBarTotalDurationChangedEvent 
        {
             TotalDuration = TotalDuration 
        });
    }
}
