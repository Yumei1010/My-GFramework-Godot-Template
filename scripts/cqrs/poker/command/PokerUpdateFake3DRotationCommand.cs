using GFramework.Core.command;
using GFramework.Core.extensions;
using TimeToTwentyfour.scripts.system.poker;

namespace TimeToTwentyfour.scripts.cqrs.poker.command;

public sealed class PokerUpdateFake3DRotationCommand : AbstractCommand
{
    public Guid PokerId {get; init; }
    protected override void OnExecute()
    {
        this.GetSystem<PokerAnimationSystem>().UpdateFake3DRotation(PokerId);
    }
}