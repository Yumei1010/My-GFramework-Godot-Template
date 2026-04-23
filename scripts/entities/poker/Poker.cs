using GFramework.Core.Abstractions.controller;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using GFrameworkGodotTemplate.scripts.enums.poker;
using GFrameworkGodotTemplate.scripts.enums.resources;
using Godot;

namespace GFrameworkGodotTemplate.scripts.entities.poker;

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
    
    public override void _Process(double delta)
    {
        StateMachine.Process(delta);
        
        Vector2 shadowPos = ShadowRect.Position;
        shadowPos.X = Mathf.Lerp(0f, -Mathf.Sign(GetGlobalPosition().X - (GetViewportRect().Size / 2f).X) * 20f, Mathf.Abs((GetGlobalPosition().X - (GetViewportRect().Size / 2f).X) / (GetViewportRect().Size / 2f).X));
        ShadowRect.Position = shadowPos;
    }

    public override void _GuiInput(InputEvent @event)
    {
        float rotX = Mathf.RadToDeg(Mathf.LerpAngle(Mathf.DegToRad(-5f), Mathf.DegToRad(5f), Mathf.Clamp(GetLocalMousePosition().X / Size.X, 0f, 1f)));
        float rotY = Mathf.RadToDeg(Mathf.LerpAngle(Mathf.DegToRad(5f), Mathf.DegToRad(-5f), Mathf.Clamp(GetLocalMousePosition().Y / Size.Y, 0f, 1f)));
        _material.SetShaderParameter("x_rot", rotY);
        _material.SetShaderParameter("y_rot", rotX);
    }
    
    public Guid GetId()
    {
        return Id;
    }

    public SuitType GetSuitType()
    {
        return SuitType;
    }

    public string GetNumValue()
    {
        return NumValue;
    }

    public NumType GetNumType()
    {
        return NumType;
    }

    public Vector2 GetSpawnPosition()
    {
        return DefaultPosition;
    }

    public void SetSuitType(SuitType suitType)
    {
        SuitType = suitType;
    }

    public void SetNumValue(string numValue)
    {
        NumValue = numValue;
    }

    public void SetNumType(NumType numType)
    {
        NumType = numType;
    }

    public void SetGlobalPosition(Vector2 pos)
    {
        GlobalPosition = pos;
    }

    public void SetDefaultRotation(float angle)
    {
        DefaultRotation = angle;
    }
    
    public void SetDefaultPosition(Vector2 pos)
    {
        DefaultPosition = pos;
    }
    
    public void ResetPos()
    {
        // 如果正在播放动画，使其终止
        if (!_tweenPos.IsNull() && _tweenPos.IsRunning()) _tweenPos.Kill();
        
        _tweenPos = CreateTween().SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Elastic);
        _tweenPos.TweenProperty( this, "global_position", DefaultPosition, 0.25f);
    }
    
    public void ResetRot()
    {
        // 如果正在播放动画，使其终止
        if (!_tweenRot.IsNull() && _tweenRot.IsRunning()) _tweenRot.Kill();
        
        _tweenRot = CreateTween().SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Elastic);
        _tweenRot.TweenProperty(this, "rotation", Mathf.DegToRad(DefaultRotation), 0.25f);
    }

    public void ResetPosAndRot()
    {
        ResetPos();
        ResetRot();
    }

    public void ChangeTo(StateType state)
    {
        StateMachine.ChangeTo(state);
    }
    
    private void UpdateNumValueLabel()
    {
        if (NumValue != "") NumLabel.Text = NumValue;
    }

    private void UpdateSurfaceRect()
    {
        SurfaceRect.Texture = SuitType switch
        {
            SuitType.Heart => _textureRegistry.Get(nameof(TextureKey.PokerSuitHeart)) as Texture2D,
            SuitType.Diamond => _textureRegistry.Get(nameof(TextureKey.PokerSuitDiamond)) as Texture2D,
            SuitType.Spade => _textureRegistry.Get(nameof(TextureKey.PokerSuitSpade)) as Texture2D,
            SuitType.Club => _textureRegistry.Get(nameof(TextureKey.PokerSuitClub)) as Texture2D,
            _ => null
        };
    }
}