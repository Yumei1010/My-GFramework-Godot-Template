using Godot;

namespace TimeToTwentyfour.scripts.menu.main_menu.optionButton;

/// <summary>
///     继续游戏按钮，用于恢复上一局未完成的游戏进程。
/// </summary>
public partial class ContinueOptionButton : MainMenuOptionButton
{
    public override void OnMouseDown()
    {
        
    }

    public override void OnMouseUp()
    {
        
    }

    public override void OnMouseEnter()
    {
        // 如果正在播放动画，使其终止
        if (!TweenMask.IsNull() && TweenMask.IsRunning()) TweenMask.Kill();
        
        TweenMask = CreateTween().SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Linear);
        TweenMask.TweenProperty(MaskRect, "size", new Vector2(192,32), 0.20f);
    }

    public override void OnMouseExit()
    {
        // 如果正在播放动画，使其终止
        if (!TweenMask.IsNull() && TweenMask.IsRunning()) TweenMask.Kill();
        
        TweenMask = CreateTween().SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Linear);
        TweenMask.TweenProperty(MaskRect, "size", new Vector2(0,32), 0.05f);
    }
}