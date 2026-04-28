using GFramework.Core.extensions;
using Godot;
using TimeToTwentyfour.scripts.cqrs.poker.@event;
using TimeToTwentyfour.scripts.entities.poker;

namespace TimeToTwentyfour.scripts.entities.deck;

public partial class Deck
{
    private void ConnectSignal()
    {
        HolderContainer.SortChildren += OnHolderContainerSortChildren;
    }

    private void OnHolderContainerSortChildren()
    {
        this.SendEvent(new DeckSortFinishedEvent());

        for (int i = 0; i < PokerContainer.GetChildCount(); i++)
        {
            IPoker poker = (IPoker)PokerContainer.GetChildren()[i];
            Control holder = (Control)HolderContainer.GetChildren()[i];

            Vector2 pos = holder.GetGlobalPosition() + holder.GetSize() / 2;
            Mapping[i] = new MappingData { Position = pos, Angle = 2f * i };
            poker.SetResetPosition(pos);
            poker.SpawnTo(pos);
        }
    }
}