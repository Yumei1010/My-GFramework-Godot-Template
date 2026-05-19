using GFramework.Core.command;
using GFramework.Core.extensions;
using TimeToTwentyfour.scripts.enums.deck;
using TimeToTwentyfour.scripts.model.deck;

namespace TimeToTwentyfour.scripts.cqrs.deck.command;

public sealed class DeckChangeSortModeCommand : AbstractCommand
{
    public required DeckSortMode TargetSortMode { get; set; }

    protected override void OnExecute()
    {
        this.GetModel<DeckModel>().CurrentSortMode = TargetSortMode;
    }
}