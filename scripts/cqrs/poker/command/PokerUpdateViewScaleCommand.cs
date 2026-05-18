using GFramework.Core.command;
using GFramework.Core.extensions;
using Godot;
using TimeToTwentyfour.scripts.system.poker;

namespace TimeToTwentyfour.scripts.cqrs.poker.command;

public sealed class PokerUpdateViewScaleCommand : AbstractCommand
{
    public required Guid PokerId { get; set; }
    public required Vector2 TargetScale { get; set; }

    protected override void OnExecute()
    {
        this.GetSystem<PokerAnimationSystem>().UpdateViewScale(PokerId, TargetScale);
    }
}