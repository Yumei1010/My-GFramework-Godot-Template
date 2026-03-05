using GFramework.Core.model;
using GFrameworkGodotTemplate.scripts.enums;

namespace GFrameworkGodotTemplate.scripts.data.model;

public class PlayerModel : AbstractModel , IPlayerModel
{
    public string Name { get; set; }
    public PileType CurrentPileType { get; set; }
    public int CurrentLevel { get; set; }
    public int CurrentMaxTime { get; set; }
    
    protected override void OnInit()
    {
        
    }
}