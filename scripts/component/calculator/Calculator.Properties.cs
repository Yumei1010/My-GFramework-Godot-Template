using TimeToTwentyfour.scripts.component.calculator.mode;
using TimeToTwentyfour.scripts.enums.calculator;

namespace TimeToTwentyfour.scripts.component.calculator;

public partial class Calculator
{
    private Dictionary<ModeType, IMode> Modes { get; } = new();
    private IMode PreviousMode { get; set; } = null!;
    private IMode CurrentMode { get; set; } = null!;
}
