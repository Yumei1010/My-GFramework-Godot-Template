using System;

namespace TimeToTwentyfour.scripts.model.pile;

public class HandPileModel : PileModel
{
    public override void AddCard(Card card)
    {
        Pile.Add(card);
    }

    public override void RemoveCard(Card card)
    {
        Pile.Remove(card);
    }

    public override Card GetRandomCard()
    {
        return Pile[Random.Shared.Next(Pile.Count)];
    }
}
