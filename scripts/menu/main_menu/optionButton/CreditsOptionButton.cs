using GFramework.Core.extensions;
using GFramework.SourceGenerators.Abstractions.rule;
using TimeToTwentyfour.scripts.cqrs.menu.mainMenu.@event;
using Godot;

namespace TimeToTwentyfour.scripts.menu.main_menu.optionButton;

/// <summary>
///     制作者按钮，点击后切换到制作者名单页面。
/// </summary>
[ContextAware]
public partial class CreditsOptionButton : MainMenuOptionButton
{
    public override void OnMouseDown()
    {
        this.SendEvent(new MainMenuCreditsButtonClickedEvent());
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