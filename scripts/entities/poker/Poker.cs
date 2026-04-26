using GFramework.Core.Abstractions.controller;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using TimeToTwentyfour.scripts.enums.poker;
using TimeToTwentyfour.scripts.enums.resources;
using Godot;

namespace TimeToTwentyfour.scripts.entities.poker;

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
        StateMachine.GuiInput(@event);
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

    public bool GetFake3D()
    {
        return Fake3D;
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

    public void ChangeTo(StateType state)
    {
        StateMachine.ChangeTo(state);
    }

    public void SpawnTo(Vector2 position)
    {
        GlobalPosition = position;
    }
    
    public void MoveTo(Vector2 position)
    {
        if (TweenAnimate)
        {
            // 如果正在播放动画，使其终止
            if (!_tweenPos.IsNull() && _tweenPos.IsRunning()) _tweenPos.Kill();
        
            _tweenPos = CreateTween().SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Elastic);
            _tweenPos.TweenProperty( this, "global_position", position, TweenAnimateTime);
        }
        else
        {
            GlobalPosition = position;
        }
    }

    public void Reparent(Node parent)
    {
        base.Reparent(parent);
    }

    public void SetTopLevel(bool topLevel)
    {
        TopLevel = topLevel;
    }

    private void UpdateNumValueLabel()
    {
        if (!string.IsNullOrWhiteSpace(NumValue)) NumLabel.Text = NumValue;
    }

    private void UpdateSurfaceRect()
    {
        SurfaceRect.Texture = SuitType switch
        {
            SuitType.Heart => _textureRegistry.Get(nameof(TextureKey.PokerSuitHeart)) as Texture2D,
            SuitType.Diamond => _textureRegistry.Get(nameof(TextureKey.PokerSuitDiamond)) as Texture2D,
            SuitType.Spade => _textureRegistry.Get(nameof(TextureKey.PokerSuitSpade)) as Texture2D,
            SuitType.Club => _textureRegistry.Get(nameof(TextureKey.PokerSuitClub)) as Texture2D,
            _ => throw new InvalidOperationException("didn't have this SuitType")
        };
    }

    public void SetXRotAndYRot(float rotX, float rotY)
    {
        _material.SetShaderParameter("x_rot", rotX);
        _material.SetShaderParameter("y_rot", rotY);
    }
}