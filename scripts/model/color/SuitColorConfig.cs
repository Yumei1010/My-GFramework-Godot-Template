using Godot;
using TimeToTwentyfour.scripts.enums.poker;

namespace TimeToTwentyfour.scripts.model.color;

/// <summary>
///     扑克花色静态色值配置表，提供各花色的主调色与副调色。
/// </summary>
public static class SuitColorConfig
{
    /// <summary>获取指定花色的主调色 <see cref="Color"/>。</summary>
    public static Color GetPrimary(SuitType suit) => suit switch
    {
        SuitType.Heart => new Color("#f0275e"),
        SuitType.Diamond => new Color("#E8A33A"),
        SuitType.Spade => new Color("#323232"),
        SuitType.Club => new Color("#009b8b"),
        _ => Colors.White
    };

    /// <summary>获取指定花色的副调色 <see cref="Color"/>。</summary>
    public static Color GetSecondary(SuitType suit) => suit switch
    {
        SuitType.Heart => new Color("#f9003b"),
        SuitType.Diamond => new Color("#ffbb00"),
        SuitType.Spade => new Color("#1a1a1a"),
        SuitType.Club => new Color("#129e67"),
        _ => Colors.White
    };
}
