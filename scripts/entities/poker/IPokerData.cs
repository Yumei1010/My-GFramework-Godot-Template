using TimeToTwentyfour.scripts.enums.poker;

namespace TimeToTwentyfour.scripts.entities.poker;

/// <summary>
///     扑克牌纯数据契约
///     仅包含扑克的身份标识与牌面数据，不涉及任何视图或 Godot 依赖。
/// </summary>
public interface IPokerData
{
    /// <summary>唯一标识符。</summary>
    Guid Id { get; set; }

    /// <summary>花色类型。</summary>
    PokerSuitType PokerSuitType { get; set; }

    /// <summary>点数数值。</summary>
    string NumValue { get; set; }

    /// <summary>点数类型。</summary>
    PokerNumType PokerNumType { get; set; }
}
