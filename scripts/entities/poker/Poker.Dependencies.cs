using GFramework.Core.extensions;
using Godot;
using TimeToTwentyfour.scripts.enums.poker;
using TimeToTwentyfour.global;
using TimeToTwentyfour.scripts.cqrs.poker.command;

namespace TimeToTwentyfour.scripts.entities.poker;

public partial class Poker
{
    private TextureRect ShadowRect => GetNode<TextureRect>("%ShadowRect");
    private TextureRect SurfaceRect => GetNode<TextureRect>("%SurfaceRect");
    private Label NumLabel => GetNode<Label>("%NumLabel");

    private async Task ReadyAsync()
    {
        await GameEntryPoint.Architecture.WaitUntilReadyAsync().ConfigureAwait(false);

        this.SendCommand(new PokerInitStateBundleCommand{ Poker = this });
        this.SendCommand(new PokerInitAnimationBundleCommand{ Poker = this, Material = (ShaderMaterial)SurfaceRect.Material, ShadowRect = ShadowRect });
        this.SendCommand(new PokerInitThemeBundleCommand{ PokerId = Id, Material = (ShaderMaterial)SurfaceRect.Material, SurfaceRect = SurfaceRect, NumLabel = NumLabel, SuitType = SuitType, NumValue = NumValue });

        ChangeTo(StateType.Idle);
    }
}
