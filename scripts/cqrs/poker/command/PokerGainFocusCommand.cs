using GFramework.Core.command;
using GFramework.Core.extensions;
using TimeToTwentyfour.scripts.system.Poker;

namespace TimeToTwentyfour.scripts.cqrs.poker.command;

public sealed class PokerGainFocusCommand : AbstractCommand
{
    public Guid PokerId {get; init; }

    protected override void OnExecute()
    {
        this.GetSystem<PokerStateSystem>().MouseEnter(PokerId);
    }
}
