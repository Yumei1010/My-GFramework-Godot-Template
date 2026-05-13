using Godot;

namespace TimeToTwentyfour.scripts.entities.poker;

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
        _manager.MouseDown(Id);
    }

    private void OnButtonUp()
    {
        _manager.MouseUp(Id);
    }

    private void OnMouseEntered()
    {
        _manager.MouseEnter(Id);
    }

    private void OnMouseExited()
    {
        if (Fake3D)
        {
            // 如果正在播放动画，使其终止
            if (!_tweenRot.IsNull() && _tweenRot.IsRunning()) _tweenRot.Kill();

            _tweenRot = CreateTween().SetParallel().SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Back);
            _tweenRot.TweenProperty(_material, "shader_parameter/x_rot", 0.0f, AnimateTime);
            _tweenRot.TweenProperty(_material, "shader_parameter/y_rot", 0.0f, AnimateTime);
        }

        _manager.MouseExit(Id);
    }
}