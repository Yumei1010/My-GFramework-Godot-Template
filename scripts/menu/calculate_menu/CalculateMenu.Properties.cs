using GFramework.Game.Abstractions.ui;
using TimeToTwentyfour.scripts.enums.ui;

namespace TimeToTwentyfour.scripts.menu.calculate_menu;

public partial class CalculateMenu
{
    private IUiPageBehavior? _page;

    /// <summary>此页面的 UI 键字符串，供 <see cref="IUiRouter"/> 路由使用。</summary>
    public static string UiKeyStr => nameof(UiKey.CalculateMenu);
}