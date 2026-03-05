using GFramework.Core.Abstractions.model;

namespace GFrameworkGodotTemplate.scripts.data.model;

public interface IPileModel : IModel
{
    float FlowRate {get; set; } //流逝
    int Resilience {get; set; } //韧性
    int Conversion {get; set; } //转化
    int Drain {get; set; } //汲取
    int Regeneration {get; set; } //再生
    int Rewind {get; set; } //回溯
    
    int FlowRateLevel  {get; set; }
    int ResilienceLevel {get; set; }
    int ConversionLevel {get; set; }
    int DrainLevel {get; set; }
    int RegenerationLevel {get; set; }
    int RewindLevel {get; set; }
    
    String Introduction {get; set; }
}