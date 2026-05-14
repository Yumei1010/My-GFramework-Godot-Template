using Godot;
using TimeToTwentyfour.scripts.enums.poker;

namespace TimeToTwentyfour.scripts.entities.poker.theme;

public class PokerTheme : IPokerTheme
{
    public ThemeType Theme { get; set; }
    public IPokerView Poker { get; set; }
    public Color SuitThemeColor { get; set; }
    public Color TextColor { get; set; }

    public virtual void Applay() {}

    public virtual void Reset() {}
}
