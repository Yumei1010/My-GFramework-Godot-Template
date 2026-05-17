using GFramework.Core.Abstractions.enums;
using GFramework.Core.Abstractions.system;
using GFramework.Core.extensions;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using Godot;
using TimeToTwentyfour.scripts.entities.poker.theme;
using TimeToTwentyfour.scripts.enums.poker;
using TimeToTwentyfour.scripts.utility;

namespace TimeToTwentyfour.scripts.system.Poker;

[Log]
[ContextAware]
public partial class PokerThemeSystem : ISystem
{
    private Dictionary<PokerThemeType, IPokerTheme> _themeRegistry = [];
    private IGodotTextureRegistry _textureRegistry = null!;

    private struct ThemeBundle
    {
        public ShaderMaterial Material;
        public TextureRect SurfaceRect;
        public Label NumLabel;
    }

    private Dictionary<Guid, ThemeBundle> Bundles = [];

    public void OnArchitecturePhase(ArchitecturePhase phase)
    {
        _log.Debug("System initialized: PokerThemeSystem");
    }

    public void Init()
    {
        _textureRegistry = this.GetUtility<IGodotTextureRegistry>()!;

        _themeRegistry = new()
        {
            { PokerThemeType.Heart, new HeartTheme() },
            { PokerThemeType.Diamond, new DiamondTheme() },
            { PokerThemeType.Spade, new SpadeTheme() },
            { PokerThemeType.Club, new ClubTheme() },
        };
    }

    public void Destroy()
    {
        
    }

    public void Register(Guid id, ShaderMaterial material, TextureRect surfaceRect, Label numLabel)
    {
        Bundles[id] = new ThemeBundle
        {
            Material = material,
            SurfaceRect = surfaceRect,
            NumLabel = numLabel,
        };
    }

    public void RemoveBundle(Guid id) => Bundles.Remove(id);

    public void UpdateSurface(Guid id, PokerSuitType suit)
    {
        if (!Bundles.TryGetValue(id, out var bundle)) return;

        var themeType = MapSuitToTheme(suit);
        var theme = _themeRegistry[themeType];
        var texture = _textureRegistry.Get(theme.SurfaceMaskTextureKey.ToString()) as Texture2D;

        theme.Applay(bundle.Material, texture!);
        bundle.SurfaceRect.Texture = texture;
        Bundles[id] = bundle;
    }

    public void UpdateText(Guid id, string numValue)
    {
        if (!Bundles.TryGetValue(id, out var bundle)) return;
        if (!string.IsNullOrWhiteSpace(numValue))
            bundle.NumLabel.Text = numValue;
    }

    public void UpdateTheme(Guid id, PokerSuitType suit, string numValue)
    {
        UpdateSurface(id, suit);
        UpdateText(id, numValue);
    }

    private static PokerThemeType MapSuitToTheme(PokerSuitType suit) => suit switch
    {
        PokerSuitType.Heart => PokerThemeType.Heart,
        PokerSuitType.Diamond => PokerThemeType.Diamond,
        PokerSuitType.Spade => PokerThemeType.Spade,
        PokerSuitType.Club => PokerThemeType.Club,
        _ => PokerThemeType.Heart
    };
}
