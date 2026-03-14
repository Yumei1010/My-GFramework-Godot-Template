using Godot;

namespace GFrameworkGodotTemplate.scripts.core.resource;

[GlobalClass]
public partial class PileAttributeConfig : Resource
{
    [Export] public Color ThemeColor;
    [Export] public Texture2D PileTexture = null!;
    
    [Export] public String PileName = null!;
    [Export] public string AbilityName = null!;
    [Export] public string Introduction = null!;
    
    [Export] public Texture2D FlowRateIcon = null!;
    [Export] public float FlowRate; //流逝
    [Export] public int FlowRateLevel;
    
    [Export] public Texture2D ResilienceIcon = null!;
    [Export] public int Resilience; //韧性
    [Export] public int ResilienceLevel;
    
    [Export] public Texture2D ConversionIcon = null!;
    [Export] public int Conversion; //转化
    [Export] public int ConversionLevel;
    
    [Export] public Texture2D DrainIcon = null!;
    [Export] public int Drain; //汲取
    [Export] public int DrainLevel;
    
    [Export] public Texture2D RegenerationIcon = null!;
    [Export] public int Regeneration; //再生
    [Export] public int RegenerationLevel;
    
    [Export] public Texture2D RewindIcon = null!;
    [Export] public int Rewind; //回溯
    [Export] public int RewindLevel;
}