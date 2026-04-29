using TimeToTwentyfour.scripts.utility;

namespace TimeToTwentyfour.scripts.entities.poker.mode;

public partial class PowerMode : PokerSelectorMode
{
    public override void Calculate()
    {
        Godot.GD.Print(
            Pokers[0].NumValue,
            "^", 
            Pokers[1].NumValue,
            "=",
            CalculateHelper.Calculate(Pokers[0], Pokers[1], Mode)
        );
    }

    public override string GetReserveResult()
    {
        return CalculateHelper.Calculate(Pokers[0], Pokers[1], Mode);
    }
}