using GFramework.Core.command;
using GFramework.Core.extensions;
using Godot;
using TimeToTwentyfour.scripts.enums.poker;
using TimeToTwentyfour.scripts.system.poker;

namespace TimeToTwentyfour.scripts.cqrs.poker.command;

public sealed class PokerInitThemeBundleCommand : AbstractCommand
{
    public required Guid PokerId { get; set; }
    public required ShaderMaterial Material { get; set; }
    public required TextureRect SurfaceRect { get; set; }
    public required Label NumLabel { get; set; }
    public required PokerSuitType PokerSuitType { get; set; }
    public required string NumValue { get; set; }

    protected override void OnExecute()
    {
        var system = this.GetSystem<PokerThemeSystem>();
        system.Register(PokerId, Material, SurfaceRect, NumLabel);
        system.UpdateTheme(PokerId, PokerSuitType, NumValue);
    }
}
