using GFramework.Core.command;
using GFramework.Core.extensions;
using TimeToTwentyfour.scripts.enums.poker;
using TimeToTwentyfour.scripts.system.poker;

namespace TimeToTwentyfour.scripts.cqrs.poker.command;

public sealed class PokerUpdateThemeCommand : AbstractCommand
{
    public required Guid PokerId { get; set; }
    public required PokerSuitType PokerSuitType { get; set; }
    public required string NumValue { get; set; }

    protected override void OnExecute()
    {
        this.GetSystem<PokerThemeSystem>().UpdateTheme(PokerId, PokerSuitType, NumValue);
    }
}
