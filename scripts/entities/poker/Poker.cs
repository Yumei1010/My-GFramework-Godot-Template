using GFramework.Core.Abstractions.controller;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using TimeToTwentyfour.scripts.enums.poker;
using TimeToTwentyfour.scripts.enums.resources;
using TimeToTwentyfour.scripts.model.color;
using Godot;

namespace TimeToTwentyfour.scripts.entities.poker;

/// <summary>
///     扑克契约实现类
/// </summary>
[Log]
[ContextAware]
public partial class Poker : Button, IPoker, IController
{
    public override void _Ready()
    {
        _ = ReadyAsync();
        ConnectSignal();
        RegisterEvent();
    }

    public override void _ExitTree()
    {
        PokerRegistry.Unregister(Id);
    }

    public override void _Process(double delta)
    {
        if (Shadow)
        {
            ShadowRect.Show();
            
            Vector2 shadowPos = ShadowRect.Position;
            shadowPos.X = Mathf.Lerp(0f, -Mathf.Sign(GetGlobalPosition().X - (GetViewportRect().Size / 2f).X) * 20f, Mathf.Abs((GetGlobalPosition().X - (GetViewportRect().Size / 2f).X) / (GetViewportRect().Size / 2f).X));
            ShadowRect.Position = shadowPos;
        }
        else
        {
            ShadowRect.Hide();
        }
        
        StateMachine.Process(delta);
    }

    public override void _GuiInput(InputEvent @event)
    {
        if (Fake3D)
        {
            float rotX = Mathf.RadToDeg(Mathf.LerpAngle(Mathf.DegToRad(-5f), Mathf.DegToRad(5f), Mathf.Clamp(GetLocalMousePosition().X / GetSize().X, 0f, 1f)));
            float rotY = Mathf.RadToDeg(Mathf.LerpAngle(Mathf.DegToRad(5f), Mathf.DegToRad(-5f), Mathf.Clamp(GetLocalMousePosition().Y / GetSize().Y, 0f, 1f)));
            _material.SetShaderParameter("x_rot", rotX);
            _material.SetShaderParameter("y_rot", rotY);
        }

        StateMachine.GuiInput(@event);
    }

    public void Reparent(Node parent)
    {
        base.Reparent(parent);
    }
    
    public void ChangeTo(StateType state)
    {
        StateMachine.ChangeTo(state);
    }

    public void SpawnTo(Vector2 position)
    {
        GlobalPosition = position - Size / 2;
    }
    
    public void MoveTo(Vector2 position)
    {
        if (Animate)
        {
            // 如果正在播放动画，使其终止
            if (!_tweenPos.IsNull() && _tweenPos.IsRunning()) _tweenPos.Kill();
            
            _tweenPos = CreateTween().SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Elastic);
            _tweenPos.TweenProperty( this, "global_position", position, AnimateTime);
        }
        else
        {
            GlobalPosition = position;
        }
    }

    public void Reset(string attributeName)
    {
        switch (attributeName)
        {
            case "Position":
                if (Animate)
                {
                    // 如果正在播放动画，使其终止
                    if (!_tweenPos.IsNull() && _tweenPos.IsRunning()) _tweenPos.Kill();
            
                    _tweenPos = CreateTween().SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Elastic);
                    _tweenPos.TweenProperty( this, "global_position", ResetPosition, AnimateTime);
                }
                else
                {
                    GlobalPosition = ResetPosition;
                }
                break;
            case "Rotation":
                if (Animate)
                {
                    // 如果正在播放动画，使其终止
                    if (!_tweenRot.IsNull() && _tweenRot.IsRunning()) _tweenRot.Kill();
            
                    _tweenRot = CreateTween().SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Elastic);
                    _tweenRot.TweenProperty( this, "rotation", ResetRotation, AnimateTime);
                }
                else
                {
                    GlobalPosition = ResetPosition;
                }
                break;
        }
    }

    private void UpdateNumValueLabel()
    {
        if (!string.IsNullOrWhiteSpace(NumValue)) NumLabel.Text = NumValue;
    }

    private void UpdateSurfaceRect()
    {
        SurfaceRect.Texture = SuitType switch
        {
            SuitType.Heart => _textureRegistry.Get(nameof(TextureKey.PokerSurfaceHeartMaskTexture)) as Texture2D,
            SuitType.Diamond => _textureRegistry.Get(nameof(TextureKey.PokerSurfaceDiamondMaskTexture)) as Texture2D,
            SuitType.Spade => _textureRegistry.Get(nameof(TextureKey.PokerSurfaceSpadeMaskTexture)) as Texture2D,
            SuitType.Club => _textureRegistry.Get(nameof(TextureKey.PokerSurfaceClubMaskTexture)) as Texture2D,
            _ => throw new InvalidOperationException("didn't have this SuitType")
        };
        _material.SetShaderParameter("modulate_color", SuitColorConfig.GetPrimary(SuitType));
    }
}