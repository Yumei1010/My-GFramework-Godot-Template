using GFramework.Core.command;
using GFramework.Core.extensions;
using TimeToTwentyfour.scripts.cqrs.timeBar.@event;

namespace TimeToTwentyfour.scripts.cqrs.timeBar.command;

public sealed class TimeBarStopCommand : AbstractCommand
{
    protected override void OnExecute()
    {
        this.SendEvent(new TimeBarStopedEvent());
    }
}
