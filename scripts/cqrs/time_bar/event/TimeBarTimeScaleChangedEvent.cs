namespace TimeToTwentyfour.scripts.cqrs.time_bar.@event;

public sealed class TimeBarTimeScaleChangedEvent
{
    public required float TimeScale { get; init; }
}
