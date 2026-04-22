using Godot;

namespace GFrameworkGodotTemplate.scripts.poker;

public partial class Poker
{
    private void ConnectSignal()
    {
        ButtonDown += OnButtonDown;
        ButtonUp += OnButtonUp;
        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExited;
    }
    
    private void OnButtonDown()
    {
        StateMachine.MouseDown();
    }
    
    private void OnButtonUp()
    {
        StateMachine.MouseUp();
    }

    private void OnMouseEntered()
    {
        StateMachine.MouseEnter();
                
        // 如果正在播放动画，使其终止
        if (!_tweenScale.IsNull() && _tweenScale.IsRunning()) _tweenScale.Kill();
        _tweenScale = CreateTween().SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Elastic);
        _tweenScale.TweenProperty(this, "scale", new Vector2(1.2f,1.2f), 0.25f);
    }

    private void OnMouseExited()
    {
        StateMachine.MouseExit();
        
        // 如果正在播放动画，使其终止
        if (!_tweenRot.IsNull() && _tweenRot.IsRunning()) _tweenRot.Kill();
        _tweenRot = CreateTween().SetParallel().SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Back);
        _tweenRot.TweenProperty(_material, "shader_parameter/x_rot", 0.0f, 0.5f);
        _tweenRot.TweenProperty(_material, "shader_parameter/y_rot", 0.0f, 0.5f);
        
        // 如果正在播放动画，使其终止
        if (!_tweenScale.IsNull() && _tweenScale.IsRunning()) _tweenScale.Kill();
        _tweenScale = CreateTween().SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Elastic);
        _tweenScale.TweenProperty(this, "scale", new Vector2(1.0f,1.0f), 0.25f);
    }
}