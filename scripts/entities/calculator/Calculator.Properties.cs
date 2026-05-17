using TimeToTwentyfour.scripts.entities.calculator.mode;
using TimeToTwentyfour.scripts.enums.calculator;

namespace TimeToTwentyfour.scripts.entities.calculator;

/// <summary>
///     <see cref="Calculator"/> 的属性和字段定义文件。
/// </summary>
public partial class Calculator
{
    public ModeType CurrentModeType => CurrentMode.ModeType;
    internal IMode? PreviousMode { get; set; }

    private Dictionary<ModeType, IMode> Modes { get; } = new();
    private IMode CurrentMode { get; set; } = null!;
}
