using Godot;
using TimeToTwentyfour.scripts.enums.poker;
using TimeToTwentyfour.scripts.enums.resources;

namespace TimeToTwentyfour.scripts.entities.poker.theme;

public class ClubTheme : PokerTheme
{
    public override PokerThemeType Theme => PokerThemeType.Club;
    public override PokerSuitType Suit => PokerSuitType.Club;
    public override Color SuitColor => new("#009b8b");
    public override Color TextColor => Colors.White;
    public override TextureKey SurfaceMaskTextureKey => TextureKey.PokerSurfaceClubMaskTexture;
}
