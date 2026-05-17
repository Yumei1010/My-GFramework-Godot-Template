using GFramework.Core.command;
using GFramework.Core.extensions;
using TimeToTwentyfour.scripts.system.poker;

namespace TimeToTwentyfour.scripts.cqrs.poker.command;

public sealed class PokerPickUpCommand : AbstractCommand
{
    public required Guid PokerId {get; init; }

    protected override void OnExecute()
    {
        this.GetSystem<PokerStateSystem>().MouseDown(PokerId);
    }
}
