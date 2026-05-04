using TimeToTwentyfour.global;
using TimeToTwentyfour.scripts.component.calculator.mode;
using TimeToTwentyfour.scripts.enums.calculator;

namespace TimeToTwentyfour.scripts.component.calculator;

public partial class Calculator
{
    private async Task ReadyAsync()
    {
        await GameEntryPoint.Architecture.WaitUntilReadyAsync().ConfigureAwait(false);

        Modes[ModeType.Add] = new AddMode();
        Modes[ModeType.Subtract] = new SubtractMode();
        Modes[ModeType.Multiply] = new MultiplyMode();
        Modes[ModeType.Divide] = new DivideMode();
        Modes[ModeType.Modulo] = new ModuloMode();
        Modes[ModeType.Power] = new PowerMode();
        Modes[ModeType.NthRoot] = new NthRootMode();
        Modes[ModeType.AbsoluteValue] = new AbsoluteValueMode();
        Modes[ModeType.Factorial] = new FactorialMode();
        Modes[ModeType.SquareRoot] = new SquareRootMode();
        Modes[ModeType.Ceil] = new CeilMode();
        Modes[ModeType.Floor] = new FloorMode();
    }
}
