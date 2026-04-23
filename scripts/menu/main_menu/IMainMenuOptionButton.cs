using Godot;
using TimeToTwentyfour.scripts.component.optionButton;

namespace TimeToTwentyfour.scripts.menu.main_menu;

public interface IMainMenuOptionButton : IOptionButton
{
    /// <summary>
    /// 更新文本颜色时调用的方法
    /// </summary>
    /// <param name="color">目标文本颜色 <see cref="Color"/></param>
    void SetTextColor(Color color);
    
    /// <summary>
    /// 更新背景颜色时调用的方法
    /// </summary>
    /// <param name="color">目标背景颜色 <see cref="Color"/></param>
    void SetBackgroundColor(Color color);
    
    /// <summary>
    /// 更新遮罩颜色时调用的方法
    /// </summary>
    /// <param name="color">目标遮罩颜色 <see cref="Color"/></param>
    void SetMaskColor(Color color);
}