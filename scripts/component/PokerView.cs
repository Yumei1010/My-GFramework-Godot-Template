using GFramework.Core.Abstractions.controller;
using GFramework.Core.extensions;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using GFrameworkGodotTemplate.scripts.command.poker;
using GFrameworkGodotTemplate.scripts.model;
using Godot;

namespace GFrameworkGodotTemplate.scripts.component;

[Log]
[ContextAware]

public partial class PokerView : Button , IController
{
    private AnimationPlayer AnimationPlayer => GetNode<AnimationPlayer>("AnimationPlayer");
    private TextureRect Back => GetNode<TextureRect>("%Back");
    private TextureRect Surface => GetNode<TextureRect>("%Surface");
    private TextureRect Suit => GetNode<TextureRect>("%Suit");
    private Label Num => GetNode<Label>("%Num");
    
    private IPokerModel _model = null!;
    
    public override void _Ready()
    {
        _ = ReadyAsync();
        ConnectSignal();
        RegisterEvent();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_model.IsMoving)
        {
            GlobalPosition = GetGlobalMousePosition() - Size / 2;
        }
    }

    private async Task ReadyAsync()
    {
        _model = ContextAwareExtensions.GetModel<IPokerModel>(this)!;
    }
    
    private void ConnectSignal()
    {
        ButtonDown += () => this.SendCommand(new StartMoveCommand());
        ButtonUp += () => this.SendCommand(new FinishMoveCommand());
        MouseEntered += () => {if(AnimationPlayer.IsPlaying()){ AnimationPlayer.Stop();}AnimationPlayer.Play("Poker/focused");};
        MouseExited += () => {if(AnimationPlayer.IsPlaying()){ AnimationPlayer.Stop();}AnimationPlayer.Play("Poker/blured");};
    }

    private void RegisterEvent()
    {
        ContextAwareExtensions.RegisterEvent<PokerModel.ChangedValueEvent>(this, e =>
        {
            UpdateNum(e.Value);
        });
        
        ContextAwareExtensions.RegisterEvent<PokerModel.ChangedPositionEvent>(this, e =>
        {
            ResetPosition();
        });
    }
    
    private void UpdateNum(String value)
    {
        Num.Text = value;
    }
    
    private void ResetPosition()
    {
        var tween = CreateTween()
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Elastic)
            .TweenProperty(
            this,
            "global_position",
            _model.SpawnPosition - Size / 2,
            0.3f
        );
    }
}
