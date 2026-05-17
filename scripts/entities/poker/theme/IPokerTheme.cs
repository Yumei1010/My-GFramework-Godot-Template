using Godot;
using TimeToTwentyfour.scripts.enums.poker;
using TimeToTwentyfour.scripts.enums.resources;

namespace TimeToTwentyfour.scripts.entities.poker.theme;

public interface IPokerTheme
{
    PokerThemeType Theme { get; }
    PokerSuitType Suit { get; }
    Color SuitColor { get; }
    Color TextColor { get; }
    TextureKey SurfaceMaskTextureKey { get; }


    void Applay(ShaderMaterial material, Texture2D surfaceMask);


    void Reset(ShaderMaterial material);
}
