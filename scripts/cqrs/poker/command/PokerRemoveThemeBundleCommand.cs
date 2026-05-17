using GFramework.Core.command;
using GFramework.Core.extensions;
using TimeToTwentyfour.scripts.system.poker;

namespace TimeToTwentyfour.scripts.cqrs.poker.command;

public sealed class PokerRemoveThemeBundleCommand : AbstractCommand
{
    public required Guid PokerId { get; set; }

    protected override void OnExecute()
    {
        this.GetSystem<PokerThemeSystem>().RemoveBundle(PokerId);
    }
}
