using GFramework.Core.command;
using GFramework.Core.extensions;
using TimeToTwentyfour.scripts.system.poker;

namespace TimeToTwentyfour.scripts.cqrs.poker.command;

public sealed class PokerProcessUpdateCommand : AbstractCommand
{
    public required Guid PokerId {get; set; }
    public required double Delta {get; set; }

    protected override void OnExecute()
    {
        this.GetSystem<PokerStateSystem>().Process(PokerId, Delta);
    }
}
