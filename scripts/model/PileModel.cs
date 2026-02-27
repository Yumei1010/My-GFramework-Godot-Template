using GFramework.Core.model;

namespace GFrameworkGodotTemplate.scripts.model;

public class PileModel : AbstractModel , IPileModel
{
    public float FlowRate { get; set; }
    public int Resilience { get; set; }
    public int Conversion { get; set; }
    public int Drain { get; set; }
    public int Regeneration { get; set; }
    public int Rewind { get; set; }
    
    public int FlowRateLevel { get; set; }
    public int ResilienceLevel { get; set; }
    public int ConversionLevel { get; set; }
    public int DrainLevel { get; set; }
    public int RegenerationLevel { get; set; }
    public int RewindLevel { get; set; }
    
    public string Introduction { get; set; }
    
    protected override void OnInit()
    {
        
    }
}