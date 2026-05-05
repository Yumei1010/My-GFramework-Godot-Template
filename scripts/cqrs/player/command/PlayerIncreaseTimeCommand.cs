using GFramework.Core.command;
using GFramework.Core.extensions;
using TimeToTwentyfour.scripts.model.player;

namespace TimeToTwentyfour.scripts.cqrs.player.command;

public class PlayerIncreaseTimeCommand : AbstractCommand
{
    public double TimeToIncrease { get; init; }
    
    protected override void OnExecute()
    {
        var model = this.GetModel<PlayerModel>()!;
        model.TotalTime += TimeToIncrease;
    }
}