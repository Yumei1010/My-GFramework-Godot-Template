using GFramework.Core.command;
using GFramework.Core.extensions;
using TimeToTwentyfour.scripts.system.poker;

namespace TimeToTwentyfour.scripts.cqrs.poker.command;

public sealed class PokerUpdateShadowPositionCommand : AbstractCommand
{
    public required Guid PokerId {get; init; }

    protected override void OnExecute()
    {
        this.GetSystem<PokerAnimationSystem>().UpdateShadowPosition(PokerId);
    }
}