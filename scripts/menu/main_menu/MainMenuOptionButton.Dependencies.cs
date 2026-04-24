using TimeToTwentyfour.global;
using Godot;

namespace TimeToTwentyfour.scripts.menu.main_menu;

public partial class MainMenuOptionButton
{
    protected ColorRect BackgroundRect => GetNode<ColorRect>("%BackgroundRect");
    protected ColorRect MaskRect => GetNode<ColorRect>("%MaskRect");
    protected Label TextLabel => GetNode<Label>("%TextLabel");

    protected Tween _tweenMask = null!;
    
    private async Task ReadyAsync()
    {
        await GameEntryPoint.Architecture.WaitUntilReadyAsync().ConfigureAwait(false);
        
        // 更新文本内容
        UpdateTextLabel();
        
        // 更新文本颜色
        UpdateTextColor();
        
        // 更新背景颜色
        UpdateBackgroundRect();
        
        // 更新遮罩颜色
        UpdateMaskRect();
    }
}