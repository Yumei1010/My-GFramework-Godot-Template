using TimeToTwentyfour.scripts.enums.calculator;

namespace TimeToTwentyfour.scripts.cqrs.modeButton.@event;

public sealed class ModeButtonClickedEvent
{
    public ModeType ModeType { get; init; }
}