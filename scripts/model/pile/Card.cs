using System;
using TimeToTwentyfour.scripts.enums.poker;

namespace TimeToTwentyfour.scripts.model.pileModel;

public sealed record Card(Guid Id, SuitType SuitType, string NumValue, NumType NumType)
{
    public Guid Id { get; init; } = Id;
    public SuitType SuitType { get; init; } = SuitType;
    public string NumValue { get; init; } = NumValue;
    public NumType NumType { get; init; } = NumType;
}
