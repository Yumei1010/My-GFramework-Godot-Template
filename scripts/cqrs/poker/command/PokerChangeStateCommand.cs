using GFramework.Core.command;
using GFramework.Core.extensions;
using TimeToTwentyfour.scripts.enums.poker;
using TimeToTwentyfour.scripts.system.Poker;

namespace TimeToTwentyfour.scripts.cqrs.poker.command;

public sealed class PokerChangeStateCommand : AbstractCommand
{
    public Guid PokerId {get; init; }
    public PokerStateType State {get; init; }

    protected override void OnExecute()
    {
        this.GetSystem<PokerStateSystem>().ChangeTo(PokerId,State);
    }
}
