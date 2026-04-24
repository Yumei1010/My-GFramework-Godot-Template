using Godot;
using TimeToTwentyfour.scripts.component.optionButton;

namespace TimeToTwentyfour.scripts.menu.calculate_menu;

/// <summary>
/// 计算页面选项按钮接口，定义了计算页面选项按钮的基本属性和必须实现的功能
/// </summary>
public interface ICalculateMenuOptionButton : IOptionButton
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
}