using GFramework.Core.command;
using GFramework.Core.extensions;
using TimeToTwentyfour.scripts.system.poker;

namespace TimeToTwentyfour.scripts.cqrs.poker.command;

public sealed class PokerProcessUpdateCommand : AbstractCommand
{
    public Guid PokerId {get; init; }
    public double Delta {get; init; }

    protected override void OnExecute()
    {
        this.GetSystem<PokerStateSystem>().Process(PokerId, Delta);
    }
}
