using GFramework.Core.command;
using GFramework.Core.extensions;
using TimeToTwentyfour.scripts.cqrs.calculator.@event;
using TimeToTwentyfour.scripts.enums.calculator;
using TimeToTwentyfour.scripts.model.calculator;

namespace TimeToTwentyfour.scripts.cqrs.calculator.command;

public sealed class CalculatorChangeModeCommand : AbstractCommand
{
    public required ModeType Mode {get; init; }

    protected override void OnExecute()
    {
        this.GetModel<CalculatorModel>().Mode = Mode;

        this.SendEvent(new CalculatorModeChangedEvent
        {
            Mode = Mode
        });
    }
}
