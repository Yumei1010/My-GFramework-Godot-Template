using GFramework.Core.Abstractions.controller;
using GFramework.Core.extensions;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using Godot;
using TimeToTwentyfour.scripts.cqrs.poker.command;
using TimeToTwentyfour.scripts.enums.poker;
using TimeToTwentyfour.scripts.enums.resources;
using TimeToTwentyfour.scripts.model.color;

namespace TimeToTwentyfour.scripts.entities.poker;

/// <summary>
///     扑克契约实现类
/// </summary>
[Log]
[ContextAware]
public partial class Poker : Button, IPoker, IController
{
    // TODO 审查重构后的Poker
    public override void _Ready()
    {
        _ = ReadyAsync();
        ConnectSignal();
        RegisterEvent();
    }

    public override void _ExitTree()
    {
        this.SendCommand(new PokerRemoveStateBundleCommand { PokerId = Id });
        this.SendCommand(new PokerRemoveAnimationBundleCommand { PokerId = Id });
    }

    public override void _Process(double delta)
    {
        _animationSystem.ProcessShadow(Id);
        this.SendCommand(new PokerProcessUpdateCommand { PokerId = Id, Delta = delta });
    }

    public override void _GuiInput(InputEvent @event)
    {
        _animationSystem.ApplyFake3DRotation(Id);
        this.SendCommand(new PokerGuiInputCommand { PokerId = Id, InputEvent = @event });
    }

    public void Reparent(Node parent)
    {
        base.Reparent(parent);
    }

    public void ChangeTo(StateType state)
    {
        this.SendCommand(new PokerChangeStateCommand{ PokerId = Id, State = state });
    }

    public void SpawnTo(Vector2 position)
    {
        GlobalPosition = position - Size / 2;
    }
    
    public void MoveTo(Vector2 position)
    {
        _animationSystem.DoMoveTo(Id, position);
    }

    public void Reset(string attributeName)
    {
        _animationSystem.DoReset(Id, attributeName, ResetPosition, ResetRotation);
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