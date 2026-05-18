using GFramework.Core.command;
using GFramework.Core.extensions;
using Godot;
using TimeToTwentyfour.scripts.entities.poker;
using TimeToTwentyfour.scripts.system.poker;

namespace TimeToTwentyfour.scripts.cqrs.poker.command;

public sealed class PokerInitAnimationBundleCommand : AbstractCommand
{
    public required IPokerView Poker {get; set; }
    public required ShaderMaterial Material {get; set; }
    public required TextureRect ShadowRect {get; set; }
    public required TextureRect SurfaceRect {get; set; }
    public required Label NumLabel {get; set; }

    protected override void OnExecute()
    {
        this.GetSystem<PokerAnimationSystem>().InitAnimations(Poker, Material, ShadowRect, SurfaceRect, NumLabel);
    }
}
