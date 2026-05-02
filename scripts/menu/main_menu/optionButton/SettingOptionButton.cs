using GFramework.Core.extensions;
using GFramework.SourceGenerators.Abstractions.rule;
using TimeToTwentyfour.scripts.cqrs.menu.mainMenu.@event;
using Godot;

namespace TimeToTwentyfour.scripts.menu.main_menu.optionButton;

[ContextAware]
public partial class SettingOptionButton : MainMenuOptionButton
{
    public override void OnMouseDown()
    {
        this.SendEvent(new MainMenuSettingButtonClickedEvent());
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