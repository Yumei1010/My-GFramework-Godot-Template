using GFramework.Core.command;
using GFramework.Core.extensions;
using TimeToTwentyfour.scripts.entities.poker;
using TimeToTwentyfour.scripts.system.Poker;

namespace TimeToTwentyfour.scripts.cqrs.poker.command;

public sealed class PokerInitStateBundleCommand : AbstractCommand
{
    public IPokerView Poker {get; init; }

    protected override void OnExecute()
    {
        this.GetSystem<PokerStateSystem>().InitStates(Poker);
    }
}
