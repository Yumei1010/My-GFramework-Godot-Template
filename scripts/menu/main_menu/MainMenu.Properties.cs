using GFramework.Game.Abstractions.ui;
using TimeToTwentyfour.scripts.enums.ui;

namespace TimeToTwentyfour.scripts.menu.main_menu;

public partial class MainMenu
{
    private IUiPageBehavior? _page;
    public static string UiKeyStr => nameof(UiKey.MainMenu);
}