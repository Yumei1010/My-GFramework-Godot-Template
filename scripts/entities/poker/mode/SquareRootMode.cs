using TimeToTwentyfour.scripts.utility;

namespace TimeToTwentyfour.scripts.entities.poker.mode;

public partial class SquareRootMode : PokerSelectorMode
{
    public override void Calculate()
    {
        Godot.GD.Print(
            "2",
            "√",
            Pokers[0].NumValue,
            "=",
            CalculateHelper.Calculate(Pokers[0], Mode)
        );
    }

    public override string GetReserveResult()
    {
        return CalculateHelper.Calculate(Pokers[0], Mode);
    }
}