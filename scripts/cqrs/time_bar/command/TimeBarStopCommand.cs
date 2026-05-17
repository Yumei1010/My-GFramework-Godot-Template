using GFramework.Core.command;
using GFramework.Core.extensions;
using TimeToTwentyfour.scripts.cqrs.time_bar.@event;

namespace TimeToTwentyfour.scripts.cqrs.time_bar.command;

public sealed class TimeBarStopCommand : AbstractCommand
{
    protected override void OnExecute()
    {
        this.SendEvent(new TimeBarStopedEvent());
    }
}
