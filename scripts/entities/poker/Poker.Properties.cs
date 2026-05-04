using TimeToTwentyfour.scripts.component.calculator.mode;

namespace TimeToTwentyfour.scripts.entities.poker;

public partial class Poker
{
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>当前 <see cref="NumValue"/> 是否与 <see cref="NumType"/> 匹配，可被正确解析。</summary>
    public bool IsValid => Mode.IsValidNumValue(NumValue, NumType);
}