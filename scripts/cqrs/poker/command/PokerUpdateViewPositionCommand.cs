using GFramework.Core.command;
using GFramework.Core.extensions;
using Godot;
using TimeToTwentyfour.scripts.system.poker;

namespace TimeToTwentyfour.scripts.cqrs.poker.command;

public sealed class PokerUpdateViewPositionCommand : AbstractCommand
{
    public required Guid PokerId { get; init; }
    public required Vector2 TargetPosition { get; init; }

    protected override void OnExecute()
    {
        this.GetSystem<PokerAnimationSystem>().UpdateViewPosition(PokerId, TargetPosition);
    }
}