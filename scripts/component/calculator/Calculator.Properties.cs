using TimeToTwentyfour.scripts.component.calculator.mode;
using TimeToTwentyfour.scripts.enums.calculator;

namespace TimeToTwentyfour.scripts.component.calculator;

public partial class Calculator
{
    public ModeType? CurrentModeType => CurrentMode?.ModeType;
    internal IMode? PreviousMode { get; set; }

    private Dictionary<ModeType, IMode> Modes { get; } = new();
    private IMode CurrentMode { get; set; } = null!;
}
