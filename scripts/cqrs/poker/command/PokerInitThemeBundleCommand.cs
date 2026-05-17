using GFramework.Core.command;
using GFramework.Core.extensions;
using Godot;
using TimeToTwentyfour.scripts.enums.poker;
using TimeToTwentyfour.scripts.system.Poker;

namespace TimeToTwentyfour.scripts.cqrs.poker.command;

public sealed class PokerInitThemeBundleCommand : AbstractCommand
{
    public required Guid PokerId { get; init; }
    public required ShaderMaterial Material { get; init; }
    public required TextureRect SurfaceRect { get; init; }
    public required Label NumLabel { get; init; }
    public required PokerSuitType PokerSuitType { get; init; }
    public required string NumValue { get; init; }

    protected override void OnExecute()
    {
        var system = this.GetSystem<PokerThemeSystem>();
        system.Register(PokerId, Material, SurfaceRect, NumLabel);
        system.UpdateTheme(PokerId, PokerSuitType, NumValue);
    }
}
