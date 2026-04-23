using Godot;

namespace TimeToTwentyfour.scripts.menu.main_menu;

public partial class MainMenuOptionButton
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
    
    /// <summary>
    /// 遮罩颜色 <see cref="Color"/>
    /// </summary>
    [Export] public required Color MaskColor { get; set; }
}