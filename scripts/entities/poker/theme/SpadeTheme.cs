using Godot;
using TimeToTwentyfour.scripts.enums.poker;
using TimeToTwentyfour.scripts.enums.resources;

namespace TimeToTwentyfour.scripts.entities.poker.theme;

public class SpadeTheme : PokerTheme
{
    public override ThemeType Theme => ThemeType.Spade;
    public override SuitType Suit => SuitType.Spade;
    public override Color SuitColor => new("#323232");
    public override Color TextColor => Colors.White;
    public override TextureKey SurfaceMaskTextureKey => TextureKey.PokerSurfaceSpadeMaskTexture;
}
