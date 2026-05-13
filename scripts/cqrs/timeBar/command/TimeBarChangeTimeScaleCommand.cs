using GFramework.Core.command;
using GFramework.Core.extensions;
using TimeToTwentyfour.scripts.cqrs.timeBar.@event;
using TimeToTwentyfour.scripts.model.timeBar;

namespace TimeToTwentyfour.scripts.cqrs.timeBar.command;

public sealed class TimeBarChangeTimeScaleCommand : AbstractCommand
{
    public float TimeScale { get; init; }

    protected override void OnExecute()
    {
        this.GetModel<TimeBarModel>().TimeScale = TimeScale;

        this.SendEvent(new TimeBarTimeScaleChangedEvent
        {
            TimeScale = TimeScale
        });
    }
}
