using GFramework.Core.Abstractions.architecture;
using GFramework.Core.Abstractions.enums;
using GFramework.Core.Abstractions.system;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;

namespace TimeToTwentyfour.scripts.system.deck;

[Log]
[ContextAware]
public partial class DeckSystem : ISystem
{
    public void OnArchitecturePhase(ArchitecturePhase phase)
    {
        _log.Debug("System initialized: DeckSystem");
    }

    public void Init()
    {
        
    }

    public void Destroy()
    {
        
    }
}
