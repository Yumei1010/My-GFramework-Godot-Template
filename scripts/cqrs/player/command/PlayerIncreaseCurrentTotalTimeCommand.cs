using GFramework.Core.command;
using GFramework.Core.extensions;
using TimeToTwentyfour.scripts.model.player;

namespace TimeToTwentyfour.scripts.cqrs.player.command;

public sealed class PlayerIncreaseCurrentTotalTimeCommand : AbstractCommand
{
    public required double TimeToIncrease { get; set; }
    
    protected override void OnExecute()
    {
        var model = this.GetModel<PlayerModel>()!;
        model.CurrentTotalTime += TimeToIncrease;
    }
}