using GFramework.Core.command;
using GFramework.Core.extensions;
using TimeToTwentyfour.scripts.system.Poker;

namespace TimeToTwentyfour.scripts.cqrs.poker.command;

public sealed class PokerResetFake3DRotationCommand : AbstractCommand
{
    public Guid PokerId {get; init; }
    protected override void OnExecute()
    {
        this.GetSystem<PokerAnimationSystem>().ResetFake3DRotation(PokerId);
    }
}