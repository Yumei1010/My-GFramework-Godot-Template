using Godot;
using TimeToTwentyfour.scripts.enums.poker;
using TimeToTwentyfour.scripts.enums.resources;

namespace TimeToTwentyfour.scripts.entities.poker.theme;

public class HeartTheme : PokerTheme
{
    public override ThemeType Theme => ThemeType.Heart;
    public override SuitType Suit => SuitType.Heart;
    public override Color SuitColor => new("#f0275e");
    public override Color TextColor => Colors.White;
    public override TextureKey SurfaceMaskTextureKey => TextureKey.PokerSurfaceHeartMaskTexture;
}
