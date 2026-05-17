using GFramework.Core.command;
using GFramework.Core.extensions;
using Godot;
using TimeToTwentyfour.scripts.system.poker;

namespace TimeToTwentyfour.scripts.cqrs.poker.command;

public sealed class PokerResetViewPositionCommand : AbstractCommand
{
    public required Guid PokerId { get; init; }
    public required Vector2 ResetPosition { get; init; }

    protected override void OnExecute()
    {
        this.GetSystem<PokerAnimationSystem>().ResetViewPosition(PokerId, ResetPosition);
    }
}