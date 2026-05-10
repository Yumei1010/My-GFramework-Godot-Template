using System;
using GFramework.Core.command;
using GFramework.Core.extensions;
using TimeToTwentyfour.scripts.enums.calculator;
using TimeToTwentyfour.scripts.model.calculator;

namespace TimeToTwentyfour.scripts.cqrs.calculator.command;

public sealed class CalculatorChangeModeCommand : AbstractCommand
{
    public ModeType mode {get; init; }

    protected override void OnExecute()
    {
        this.GetModel<CalculatorModel>().Mode = mode;
    }
}
