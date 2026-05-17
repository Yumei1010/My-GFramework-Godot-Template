using GFramework.Core.command;
using GFramework.Core.extensions;
using TimeToTwentyfour.scripts.model.player;

namespace TimeToTwentyfour.scripts.cqrs.player.command;

public sealed class PlayerDecreaseCurrentTotalTimeCommand : AbstractCommand
{
    public required double TimeToDecrease { get; set; }
    
    protected override void OnExecute()
    {
        var model = this.GetModel<PlayerModel>()!;
        model.CurrentTotalTime -= TimeToDecrease;
    }
}