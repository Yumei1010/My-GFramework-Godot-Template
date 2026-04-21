using GFrameworkGodotTemplate.scripts.enums.calculate;
using GFrameworkGodotTemplate.scripts.utility;

namespace GFrameworkGodotTemplate.scripts.poker.mode;

public partial class DivideMode : PokerSelectorMode
{
    public override void Calculate()
    {
        Godot.GD.Print(
            Pokers[0].GetNumValue(),
            "/", 
            Pokers[1].GetNumValue(),
            "=",
            CalculateHelper.Calculate(Pokers[0], Pokers[1], ModeType.Divide)
        );
    }
}