using Godot;
using TimeToTwentyfour.scripts.component.theme;
using TimeToTwentyfour.scripts.enums.poker;

namespace TimeToTwentyfour.scripts.entities.poker.theme;

public interface IPokerTheme : ITheme
{
    ThemeType Theme { get; set; }
    IPokerView Poker { get; set; }
    Color SuitThemeColor { get; set; }
    Color TextColor { get; set; }
}
