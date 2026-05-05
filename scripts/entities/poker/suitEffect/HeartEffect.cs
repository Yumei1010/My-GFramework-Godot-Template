using GFramework.Core.extensions;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using TimeToTwentyfour.scripts.cqrs.player.command;

namespace TimeToTwentyfour.scripts.entities.poker.suitEffect;

[Log]
[ContextAware]
public sealed partial class HeartEffect : SuitEffect
{
    public override void CalculationFinish()
    {
        this.SendCommand(new PlayerIncreaseTimeCommand()
        {
            TimeToIncrease = 10.0
        });
        
        _log.Info("Heart effect triggered, player increased time 10.0s");
    }
}