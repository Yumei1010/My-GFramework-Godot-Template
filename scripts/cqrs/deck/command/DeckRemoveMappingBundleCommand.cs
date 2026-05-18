using GFramework.Core.command;
using GFramework.Core.extensions;
using TimeToTwentyfour.scripts.system.deck;

namespace TimeToTwentyfour.scripts.cqrs.deck.command;

public sealed class DeckRemoveMappingBundleCommand : AbstractCommand
{
    public required Guid PokerId { get; set; }

    protected override void OnExecute()
    {
        this.GetSystem<DeckSortSystem>().RemoveBundle(PokerId);
    }
}
