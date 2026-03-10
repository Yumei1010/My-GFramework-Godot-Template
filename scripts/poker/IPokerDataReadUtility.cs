using GFramework.Core.Abstractions.utility;

namespace GFrameworkGodotTemplate.scripts.poker;

public interface IPokerDataReadUtility : IContextUtility
{
    PokerData? CurrentPokerData { get; set; }
    
    void Load();
    
    Godot.Collections.Dictionary<int, PokerDefinition> GetPokerDefinitions();
}