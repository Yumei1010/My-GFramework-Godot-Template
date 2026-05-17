using GFramework.Core.command;
using GFramework.Core.extensions;
using TimeToTwentyfour.scripts.enums.poker;
using TimeToTwentyfour.scripts.system.poker;

namespace TimeToTwentyfour.scripts.cqrs.poker.command;

public sealed class PokerChangeStateCommand : AbstractCommand
{
    public required Guid PokerId {get; init; }
    public required PokerStateType State {get; init; }

    protected override void OnExecute()
    {
        this.GetSystem<PokerStateSystem>().ChangeTo(PokerId,State);
    }
}
