using System;
using TimeToTwentyfour.scripts.enums.poker;

namespace TimeToTwentyfour.scripts.model.pile;

public class DrawPileModel : PileModel
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

    protected override void OnInit()
    {
        var pile = new List<Card>(52);
        foreach (SuitType suit in Enum.GetValues<SuitType>())
        {
            for (int i = 1; i <= 13; i++)
            {
                pile.Add(new Card(Guid.NewGuid(), suit, i.ToString(), NumType.Integer));
            }
        }
        Pile = pile;
    }
}
