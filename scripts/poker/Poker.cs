using GFramework.Core.Abstractions.controller;
using GFramework.Core.extensions;
using Godot;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using Twentyfour.scripts.model;
using Twentyfour.scripts.command.poker;

namespace Twentyfour.scripts.poker;

[Log]
[ContextAware]

public partial class Poker : Button , IController
{
    private AnimationPlayer AnimationPlayer => GetNode<AnimationPlayer>("AnimationPlayer");
    private TextureRect Back => GetNode<TextureRect>("%Back");
    private TextureRect Surface => GetNode<TextureRect>("%Surface");
    private TextureRect Suit => GetNode<TextureRect>("%Suit");
    private Label Num => GetNode<Label>("%Num");
    
    private IPokerModel _model = null!;
    
    public override void _Ready()
    {
        _model = this.GetModel<IPokerModel>()!;
        
        this.RegisterEvent<PokerModel.ChangedValueEvent>(e =>
        {
            UpdateNum(e.Value);
        });
        
        this.RegisterEvent<PokerModel.ChangedPositionEvent>(e =>
        {
            ResetPosition();
        });
        
        ButtonDown += () => this.SendCommand(new StartMoveCommand());

        ButtonUp += () => this.SendCommand(new FinishMoveCommand());

        MouseEntered += () =>
        { 
            if (AnimationPlayer.IsPlaying())
            {
                AnimationPlayer.Stop();
            }
            AnimationPlayer.Play("Poker/focused");
        };

        MouseExited += () =>
        {
            if (AnimationPlayer.IsPlaying())
            {
                AnimationPlayer.Stop();
            }
            AnimationPlayer.Play("Poker/blured");
        };
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_model.IsMoving)
        {
            GlobalPosition = GetGlobalMousePosition() - Size / 2;
        }
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
