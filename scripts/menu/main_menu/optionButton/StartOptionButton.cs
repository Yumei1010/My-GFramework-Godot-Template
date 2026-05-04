using GFramework.Core.extensions;
using GFramework.SourceGenerators.Abstractions.rule;
using TimeToTwentyfour.scripts.cqrs.menu.mainMenu.@event;
using Godot;

namespace TimeToTwentyfour.scripts.menu.main_menu.optionButton;

/// <summary>
///     开始游戏按钮，点击后从主菜单进入计算菜单开始新游戏。
/// </summary>
[ContextAware]
public partial class StartOptionButton : MainMenuOptionButton
{
    public override void OnMouseDown()
    {
        this.SendEvent(new MainMenuStartButtonClickedEvent());
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