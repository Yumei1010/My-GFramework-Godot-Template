using GFramework.Core.Abstractions.enums;
using GFramework.Core.Abstractions.system;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using Godot;
using TimeToTwentyfour.scripts.entities.poker;
using TimeToTwentyfour.scripts.entities.poker.theme;
using TimeToTwentyfour.scripts.enums.poker;

namespace TimeToTwentyfour.scripts.system.Poker;

[Log]
[ContextAware]
public partial class PokerThemeSystem : ISystem
{
    private struct ThemeBundle
    {
        public Dictionary<ThemeType, IPokerTheme> Themes;
        public IPokerTheme CurrentTheme;
        public IPokerTheme PreviousTheme;
    }

    private readonly Dictionary<Guid, ThemeBundle> Bundles = [];

    public void OnArchitecturePhase(ArchitecturePhase phase)
    {
        _log.Debug("System initialized: PokerThemeSystem");
    }

    public void Init()
    {
        
    }

    public void Destroy()
    {
        
    }
    public void InitThemes(IPokerView poker)
    {
        var themes = new Dictionary<ThemeType, IPokerTheme>
        {
            { ThemeType.Heart, new HeartTheme() },
            { ThemeType.Diamond, new DiamondTheme() },
            { ThemeType.Spade, new SpadeTheme() },
            { ThemeType.Club, new ClubTheme() }
        };

        foreach (var (type, theme) in themes)
        {
            theme.Theme = type;
            theme.Poker = poker;
        }

        Bundles[poker.Id] = new ThemeBundle { Themes = themes };
    }
}
