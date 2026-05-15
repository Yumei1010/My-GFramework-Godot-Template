using Godot;
using TimeToTwentyfour.scripts.enums.poker;
using TimeToTwentyfour.scripts.enums.resources;

namespace TimeToTwentyfour.scripts.entities.poker.theme;

public class DiamondTheme : PokerTheme
{
    public override ThemeType Theme => ThemeType.Diamond;
    public override SuitType Suit => SuitType.Diamond;
    public override Color SuitColor => new("#E8A33A");
    public override Color TextColor => Colors.White;
    public override TextureKey SurfaceMaskTextureKey => TextureKey.PokerSurfaceDiamondMaskTexture;
}
