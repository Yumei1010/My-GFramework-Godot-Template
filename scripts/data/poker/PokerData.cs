using System;
using GFramework.Game.Abstractions.data;
using TimeToTwentyfour.scripts.entities.pile;
using TimeToTwentyfour.scripts.enums.poker;

namespace TimeToTwentyfour.scripts.data.poker;

public class PokerData : IData
{
    public Guid Id { get; set; } = Guid.Empty;
    public SuitType SuitType { get; set; } = SuitType.Heart;
    public string NumValue { get; set; } = "1";
    public NumType NumType { get; set; } = NumType.Integer;
    public PileType Pile { get; set; } = PileType.Draw;

    public void Reset()
    {
        Id = Guid.Empty;
        SuitType = SuitType.Heart;
        NumValue = "1";
        NumType = NumType.Integer;
        Pile = PileType.Draw;
    }
}
