using GFramework.Core.command;
using GFramework.Core.extensions;
using Godot;
using TimeToTwentyfour.scripts.entities.poker;
using TimeToTwentyfour.scripts.system.poker;

namespace TimeToTwentyfour.scripts.cqrs.poker.command;

public sealed class PokerInitAnimationBundleCommand : AbstractCommand
{
    public required IPokerView Poker {get; init; }
    public required ShaderMaterial Material {get; init; }
    public required TextureRect ShadowRect {get; init; }

    protected override void OnExecute()
    {
        this.GetSystem<PokerAnimationSystem>().InitAnimations(Poker, Material, ShadowRect);
    }
}
