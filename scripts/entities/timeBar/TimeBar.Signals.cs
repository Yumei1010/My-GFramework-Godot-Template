namespace TimeToTwentyfour.scripts.entities.timeBar;

public partial class TimeBar
{
    private void ConnectSignal()
    {
        Timer.Timeout += OnTimerTimeout;
    }
    
    private void OnTimerTimeout()
    {
        _isPaused = false;
        _log.Debug("TimeBar OnTimerTimeout");
    }
}