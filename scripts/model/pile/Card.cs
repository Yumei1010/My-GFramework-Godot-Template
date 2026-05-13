using System;
using TimeToTwentyfour.scripts.enums.poker;
using TimeToTwentyfour.scripts.entities.poker;

namespace TimeToTwentyfour.scripts.model.pile;

/// <summary>
///     扑克牌面纯数据记录，实现 <see cref="IPokerData"/> 以统一数据契约。
/// </summary>
public sealed record Card(Guid Id, SuitType SuitType, string NumValue, NumType NumType) : IPokerData
{
    public Guid Id { get; set; } = Id;
    public SuitType SuitType { get; set; } = SuitType;
    public string NumValue { get; set; } = NumValue;
    public NumType NumType { get; set; } = NumType;
}
