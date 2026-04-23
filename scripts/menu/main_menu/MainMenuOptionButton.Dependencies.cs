using TimeToTwentyfour.global;
using Godot;

namespace TimeToTwentyfour.scripts.menu.main_menu;

public partial class MainMenuOptionButton
{
    private ColorRect BackgroundRect => GetNode<ColorRect>("%BackgroundRect");
    private ColorRect MaskRect => GetNode<ColorRect>("%MaskRect");
    private Label TextLabel => GetNode<Label>("%TextLabel");
    
    private Tween _tweenMask = null!;
    
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