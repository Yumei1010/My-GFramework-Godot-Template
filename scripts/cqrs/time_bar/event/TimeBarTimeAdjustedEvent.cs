namespace TimeToTwentyfour.scripts.cqrs.time_bar.@event;

public sealed class TimeBarTimeAdjustedEvent
{
    public required float Time {get; init; }
}
