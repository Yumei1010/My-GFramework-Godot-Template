using GFramework.Core.extensions;
using GFramework.SourceGenerators.Abstractions.rule;
using GFrameworkGodotTemplate.scripts.enums.poker;
using GFrameworkGodotTemplate.scripts.events.pokerSelector;
using Godot;

namespace GFrameworkGodotTemplate.scripts.poker.state;

[ContextAware]
public partial class UnSelectState : PokerState
{
    private Tween _tween = null!;
    
    public override void Process(double delta)
    {
        
    }

    public override void Enter()
    {
        
    }

    public override void Exit()
    {

    }

    public override void MouseDown()
    {
        this.SendEvent(new SelectChangedEvent
        {
            Poker = Poker,
            IsSelected = true
        });
        
        RequestStateChange(StateType.OnSelect);
    }

    public override void MouseUp()
    {
        
    }

    public override void MouseEnter()
    {
        // 如果正在播放动画，使其终止
        if (!_tween.IsNull() && _tween.IsRunning()) _tween.Kill();
        
        _tween = CreateTween();
        _tween.SetEase(Tween.EaseType.InOut);
        _tween.SetTrans(Tween.TransitionType.Elastic);
        _tween.TweenProperty( Poker, "scale", new Vector2(1.05f,1.05f), 0.125f);
        _tween.TweenProperty( Poker, "scale", new Vector2(1.0f,1.0f), 0.125f);
    }
    
    public override void MouseExit()
    {
        
    }
}