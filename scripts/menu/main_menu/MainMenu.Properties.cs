using GFramework.Game.Abstractions.ui;
using TimeToTwentyfour.scripts.enums.ui;

namespace TimeToTwentyfour.scripts.menu.main_menu;

/// <summary>
///     <see cref="MainMenu"/> 的属性和字段定义文件。
/// </summary>
public partial class MainMenu
{
    private IUiPageBehavior? _page;
    public static string UiKeyStr => nameof(UiKey.MainMenu);
}