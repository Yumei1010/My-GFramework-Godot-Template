using GFramework.Core.command;
using GFramework.Core.extensions;
using Godot;
using TimeToTwentyfour.scripts.entities.poker;
using TimeToTwentyfour.scripts.system.Poker;

namespace TimeToTwentyfour.scripts.cqrs.poker.command;

public class PokerInitAnimationBundleCommand : AbstractCommand
{
    public IPokerView Poker {get; init; }
    public ShaderMaterial Material {get; init; }
    public TextureRect ShadowRect {get; init; }

    protected override void OnExecute()
    {
        this.GetSystem<PokerAnimationSystem>().InitAnimations(Poker, Material, ShadowRect);
    }
}
