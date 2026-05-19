using GFramework.Core.command;
using GFramework.Core.extensions;
using Godot;
using TimeToTwentyfour.scripts.entities.poker;
using TimeToTwentyfour.scripts.system.deck;

namespace TimeToTwentyfour.scripts.cqrs.deck.command;

public sealed class DeckInitMappingBundleCommand : AbstractCommand
{
    public required Panel Holder { get; set; }
    public required IPokerView PokerView { get; set; }

    protected override void OnExecute()
    {
        this.GetSystem<DeckSortSystem>().InitMapping(PokerView, Holder);
    }
}
