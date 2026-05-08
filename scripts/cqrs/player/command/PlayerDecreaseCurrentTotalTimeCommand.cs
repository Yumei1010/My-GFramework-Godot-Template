using GFramework.Core.command;
using GFramework.Core.extensions;
using TimeToTwentyfour.scripts.model.player;

namespace TimeToTwentyfour.scripts.cqrs.player.command;

public class PlayerDecreaseCurrentTotalTimeCommand : AbstractCommand
{
    public double TimeToDecrease { get; init; }
    
    protected override void OnExecute()
    {
        var model = this.GetModel<PlayerModel>()!;
        model.CurrentTotalTime -= TimeToDecrease;
    }
}