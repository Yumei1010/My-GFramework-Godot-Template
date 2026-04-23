using TimeToTwentyfour.scripts.utility;

namespace TimeToTwentyfour.scripts.entities.poker.mode;

public partial class FactorialMode : PokerSelectorMode
{
    public override void Calculate()
    {
        Godot.GD.Print(
            Pokers[0].GetNumValue(),
            "!", 
            "=",
            CalculateHelper.Calculate(Pokers[0], Mode)
        );
    }

    public override string GetReserveResult()
    {
        return CalculateHelper.Calculate(Pokers[0], Mode);
    }
}