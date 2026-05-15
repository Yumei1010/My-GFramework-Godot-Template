using Godot;
using TimeToTwentyfour.scripts.enums.poker;
using TimeToTwentyfour.scripts.enums.resources;

namespace TimeToTwentyfour.scripts.entities.poker.theme;

public abstract class PokerTheme : IPokerTheme
{
    public abstract ThemeType Theme { get; }
    public abstract SuitType Suit { get; }
    public abstract Color SuitColor { get; }
    public abstract Color TextColor { get; }
    public abstract TextureKey SurfaceMaskTextureKey { get; }

    public virtual void Applay(ShaderMaterial material, Texture2D surfaceMask)
    {
        material.SetShaderParameter("modulate_color", SuitColor);
    }

    public virtual void Reset(ShaderMaterial material)
    {
        material.SetShaderParameter("modulate_color", Colors.White);
    }
}
