using TimeToTwentyfour.scripts.enums.poker;

namespace TimeToTwentyfour.scripts.model.poker;

/// <summary>
///     扑克牌纯数据契约
///     仅包含扑克的身份标识与牌面数据，不涉及任何视图或 Godot 依赖。
/// </summary>
public interface IPokerData
{
    /// <summary>唯一标识符。</summary>
    Guid Id { get; }

    /// <summary>花色类型。</summary>
    SuitType SuitType { get; set; }

    /// <summary>点数数值。</summary>
    string NumValue { get; set; }

    /// <summary>点数类型。</summary>
    NumType NumType { get; set; }

    /// <summary>当前 <see cref="NumValue"/> 是否与 <see cref="NumType"/> 匹配，可被正确解析。</summary>
    bool IsValid { get; }
}
