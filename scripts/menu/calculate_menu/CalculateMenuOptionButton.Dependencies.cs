using TimeToTwentyfour.global;
using Godot;

namespace TimeToTwentyfour.scripts.menu.calculate_menu;

public partial class CalculateMenuOptionButton
{
    private ColorRect BackgroundRect => GetNode<ColorRect>("%BackgroundRect");
    private Label TextLabel => GetNode<Label>("%TextLabel");
    
    private async Task ReadyAsync()
    {
        // 等待框架加载完成
        await GameEntryPoint.Architecture.WaitUntilReadyAsync().ConfigureAwait(false);
        
        // 更新文本内容
        UpdateTextLabel();
        
        // 更新文本颜色
        UpdateTextColor();
        
        // 更新背景颜色
        UpdateBackgroundRect();
    }
}