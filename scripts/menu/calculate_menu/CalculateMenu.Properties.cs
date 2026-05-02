using GFramework.Game.Abstractions.ui;
using TimeToTwentyfour.scripts.enums.ui;

namespace TimeToTwentyfour.scripts.menu.calculate_menu;

public partial class CalculateMenu
{
    private IUiPageBehavior? _page;
    public static string UiKeyStr => nameof(UiKey.CalculateMenu);
}