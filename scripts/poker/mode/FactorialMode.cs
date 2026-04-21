using GFrameworkGodotTemplate.scripts.enums.calculate;
using GFrameworkGodotTemplate.scripts.utility;

namespace GFrameworkGodotTemplate.scripts.poker.mode;

public partial class FactorialMode : PokerSelectorMode
{
    public override void Calculate()
    {
        Godot.GD.Print(
            Pokers[0].GetNumValue(),
            "!", 
            "=",
            CalculateHelper.Calculate(Pokers[0], ModeType.Factorial)
        );
    }
}