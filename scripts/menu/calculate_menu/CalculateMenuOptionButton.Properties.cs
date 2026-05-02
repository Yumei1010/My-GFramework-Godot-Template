using Godot;

namespace TimeToTwentyfour.scripts.menu.calculate_menu;

public partial class CalculateMenuOptionButton
{
    /// <summary>
    /// 文本内容 <see cref="string"/>
    /// </summary>
    [Export] public required string TextContent { get; set; }
    
    /// <summary>
    /// 文本颜色 <see cref="Color"/>
    /// </summary>
    [Export] public required Color TextColor { get; set; }
    
    /// <summary>
    /// 背景颜色 <see cref="Color"/>
    /// </summary>
    [Export] public required Color BackgroundColor { get; set; }
}