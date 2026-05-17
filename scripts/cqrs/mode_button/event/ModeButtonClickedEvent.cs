using TimeToTwentyfour.scripts.enums.calculator;

namespace TimeToTwentyfour.scripts.cqrs.mode_button.@event;

public sealed class ModeButtonClickedEvent
{
    public required ModeType ModeType { get; init; }
}