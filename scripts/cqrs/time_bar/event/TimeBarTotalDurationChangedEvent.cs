namespace TimeToTwentyfour.scripts.cqrs.time_bar.@event;

public sealed class TimeBarTotalDurationChangedEvent
{
    public required float TotalDuration { get; init; }
}
