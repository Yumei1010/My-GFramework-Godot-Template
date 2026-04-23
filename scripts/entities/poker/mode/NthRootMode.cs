using GFrameworkGodotTemplate.scripts.utility;

namespace GFrameworkGodotTemplate.scripts.entities.poker.mode;

public partial class NthRootMode : PokerSelectorMode
{
    public override void Calculate()
    {
        Godot.GD.Print(
            Pokers[0].GetNumValue(),
            "√", 
            Pokers[1].GetNumValue(),
            "=",
            CalculateHelper.Calculate(Pokers[0], Pokers[1], Mode)
        );
    }

    public override string GetReserveResult()
    {
        return CalculateHelper.Calculate(Pokers[0], Pokers[1], Mode);
    }
}