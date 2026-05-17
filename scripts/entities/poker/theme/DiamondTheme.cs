using Godot;
using TimeToTwentyfour.scripts.enums.poker;
using TimeToTwentyfour.scripts.enums.resources;

namespace TimeToTwentyfour.scripts.entities.poker.theme;

public class DiamondTheme : PokerTheme
{
    public override PokerThemeType Theme => PokerThemeType.Diamond;
    public override PokerSuitType Suit => PokerSuitType.Diamond;
    public override Color SuitColor => new("#E8A33A");
    public override Color TextColor => Colors.White;
    public override TextureKey SurfaceMaskTextureKey => TextureKey.PokerSurfaceDiamondMaskTexture;
}
