using System;
using TimeToTwentyfour.scripts.component.calculator.mode;
using TimeToTwentyfour.scripts.enums.calculator;

namespace TimeToTwentyfour.scripts.cqrs.calculator.@event;

public sealed class CalculatorModeChangedEvent
{
    public ModeType Mode { get; init; }
}
