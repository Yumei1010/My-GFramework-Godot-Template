using GFramework.Core.Abstractions.enums;
using GFramework.Core.Abstractions.system;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;

namespace TimeToTwentyfour.scripts.system.Poker;

[Log]
[ContextAware]
public partial class PokerSuitEffectSystem : ISystem
{
    public void OnArchitecturePhase(ArchitecturePhase phase)
    {
        _log.Debug("System initialized: PokerSuitEffectSystem");
    }
    
    public void Init()
    {
        
    }

    public void Destroy()
    {
        
    }
}
