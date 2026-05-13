using GFramework.Core.extensions;
using Godot;
using TimeToTwentyfour.scripts.cqrs.poker.command;

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
        this.SendCommand(new PokerPickUpCommand { PokerId = Id });
    }

    private void OnButtonUp()
    {
        this.SendCommand(new PokerPutDownCommand { PokerId = Id });
    }

    private void OnMouseEntered()
    {
        this.SendCommand(new PokerGainFocusCommand { PokerId = Id });
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

        this.SendCommand(new PokerLoseFocusCommand { PokerId = Id });
    }
}