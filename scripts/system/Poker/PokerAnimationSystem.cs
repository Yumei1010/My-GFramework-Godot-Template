using GFramework.Core.Abstractions.enums;
using GFramework.Core.Abstractions.system;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using TimeToTwentyfour.scripts.entities.poker;

namespace TimeToTwentyfour.scripts.system.Poker;

[Log]
[ContextAware]
public partial class PokerAnimationSystem : ISystem
{
    private struct AnimationBundle
    {
        public bool Shadow;
        public bool Animate;
        public double AnimateTime;
        public bool Fake3D;
    }

    private Dictionary<Guid, AnimationBundle> Bundles = [];

    public void OnArchitecturePhase(ArchitecturePhase phase)
    {
        _log.Debug("System initialized: PokerAnimationSystem");
    }

    public void Init()
    {
        
    }

    public void Destroy()
    {
        
    }

    public void InitStates(IPokerView poker)
    {
        Bundles[poker.Id] = new AnimationBundle { Shadow = true, Animate = true, AnimateTime = 0.25, Fake3D = true };
    }
}
