using GFrameworkGodotTemplate.scripts.enums.calculate;
using GFrameworkGodotTemplate.scripts.utility;

namespace GFrameworkGodotTemplate.scripts.poker.mode;

public partial class SquareRootMode : PokerSelectorMode
{
    public override void Calculate()
    {
        Godot.GD.Print(
            "2",
            "√",
            Pokers[0].GetNumValue(),
            "=",
            CalculateHelper.Calculate(Pokers[0], ModeType.SquareRoot)
        );
    }
}