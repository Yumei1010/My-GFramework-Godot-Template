using GFramework.Core.command;
using GFramework.Core.extensions;
using TimeToTwentyfour.global;

namespace TimeToTwentyfour.scripts.cqrs.poker.command;

public sealed class PokerGainFocusCommand : AbstractCommand
{
    public Guid PokerId {get; init; }

    protected override void OnExecute()
    {
        this.GetSystem<PokerManager>().MouseEnter(PokerId);
    }
}
