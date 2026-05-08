using GFramework.Core.model;
using TimeToTwentyfour.scripts.enums.calculator;

namespace TimeToTwentyfour.scripts.model.player;

public class PlayerModel : AbstractModel
{
    public double CurrentTotalTime { get; set; } = 0.0;

    public IList<ModeType> MaxEnabledModes { get; set; } = new List<ModeType>
    {
        ModeType.Add,
        ModeType.Subtract,
        ModeType.Multiply,
        ModeType.Divide,
        ModeType.Modulo,
        ModeType.NthRoot,
        ModeType.Power,
        ModeType.Factorial,
        ModeType.AbsoluteValue,
        ModeType.Ceil,
        ModeType.Floor,
        ModeType.SquareRoot
    };

    public IList<ModeType> CurrentEnabledModes { get; set; } = new List<ModeType>
    {
        ModeType.Add,
        ModeType.Subtract,
        ModeType.Multiply,
        ModeType.Divide,
    };
    
    protected override void OnInit()
    {
        
    }
}