using GFramework.Core.command;
using GFramework.Core.extensions;
using TimeToTwentyfour.scripts.cqrs.time_bar.@event;
using TimeToTwentyfour.scripts.model.time_bar;

namespace TimeToTwentyfour.scripts.cqrs.time_bar.command;

public sealed class TimeBarChangeTimeScaleCommand : AbstractCommand
{
    public required float TimeScale { get; init; }

    protected override void OnExecute()
    {
        this.GetModel<TimeBarModel>().TimeScale = TimeScale;

        this.SendEvent(new TimeBarTimeScaleChangedEvent
        {
            TimeScale = TimeScale
        });
    }
}
