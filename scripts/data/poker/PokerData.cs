using System;
using GFramework.Game.Abstractions.data;
using TimeToTwentyfour.scripts.entities.pile;
using TimeToTwentyfour.scripts.enums.poker;

namespace TimeToTwentyfour.scripts.data.poker;

public class PokerData : IData
{
    public Guid Id { get; set; } = Guid.Empty;
    public PokerSuitType PokerSuitType { get; set; } = PokerSuitType.Heart;
    public string NumValue { get; set; } = "1";
    public PokerNumType PokerNumType { get; set; } = PokerNumType.Integer;
    public PokerPileType Pile { get; set; } = PokerPileType.Draw;

    public void Reset()
    {
        Id = Guid.Empty;
        PokerSuitType = PokerSuitType.Heart;
        NumValue = "1";
        PokerNumType = PokerNumType.Integer;
        Pile = PokerPileType.Draw;
    }
}
