using GFramework.Core.Abstractions.model;
using GFrameworkGodotTemplate.scripts.enums;

namespace GFrameworkGodotTemplate.scripts.data.model;

public interface IPlayerModel : IModel
{
    string Name {get; set; }
    
    PileType CurrentPileType {get; set; }
    
    int CurrentLevel {get; set; }
    
    int CurrentMaxTime {get; set; }
}