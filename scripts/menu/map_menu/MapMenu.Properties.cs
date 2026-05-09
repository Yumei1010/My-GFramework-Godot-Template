using GFramework.Game.Abstractions.ui;
using TimeToTwentyfour.scripts.enums.ui;

namespace TimeToTwentyfour.scripts.menu.map_menu;

/// <summary>
///     <see cref="MapMenu"/> 的属性和字段定义文件。
/// </summary>
public partial class MapMenu
{
    private IUiPageBehavior? _page;
    public static string UiKeyStr => nameof(UiKey.MainMenu);
}