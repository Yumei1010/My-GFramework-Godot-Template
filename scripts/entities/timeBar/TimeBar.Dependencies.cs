using Godot;

namespace TimeToTwentyfour.scripts.entities.timeBar;

public partial class TimeBar
{
    private Timer Timer => GetNode<Timer>("%Timer");
}