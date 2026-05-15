using GFramework.Core.command;
using GFramework.Core.extensions;
using TimeToTwentyfour.scripts.enums.poker;
using TimeToTwentyfour.scripts.system.Poker;

namespace TimeToTwentyfour.scripts.cqrs.poker.command;

public sealed class PokerUpdateThemeCommand : AbstractCommand
{
    public required Guid PokerId { get; init; }
    public required SuitType SuitType { get; init; }
    public required string NumValue { get; init; }

    protected override void OnExecute()
    {
        this.GetSystem<PokerThemeSystem>().UpdateTheme(PokerId, SuitType, NumValue);
    }
}
