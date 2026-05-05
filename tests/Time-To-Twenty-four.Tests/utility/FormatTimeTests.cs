namespace TimeToTwentyfour.Tests.utility;

public class FormatTimeTests
{
    [Fact]
    public void FormatTime_ZeroSeconds_ReturnsZeroTime()
    {
        var result = TimeBar.FormatTime(0f);
        Assert.Equal("00:00", result);
    }

    [Fact]
    public void FormatTime_NegativeSeconds_ReturnsZeroTime()
    {
        var result = TimeBar.FormatTime(-5f);
        Assert.Equal("00:00", result);
    }

    [Fact]
    public void FormatTime_OneSecond_ReturnsOneSecond()
    {
        var result = TimeBar.FormatTime(1f);
        Assert.Equal("00:01", result);
    }

    [Fact]
    public void FormatTime_FiftyNineSeconds()
    {
        var result = TimeBar.FormatTime(59f);
        Assert.Equal("00:59", result);
    }

    [Fact]
    public void FormatTime_OneMinute_ReturnsOneMinute()
    {
        var result = TimeBar.FormatTime(60f);
        Assert.Equal("01:00", result);
    }

    [Fact]
    public void FormatTime_TenMinutes()
    {
        var result = TimeBar.FormatTime(600f);
        Assert.Equal("10:00", result);
    }

    [Fact]
    public void FormatTime_FractionalSecond_CeilsUp()
    {
        var result = TimeBar.FormatTime(59.1f);
        Assert.Equal("01:00", result);
    }

    [Fact]
    public void FormatTime_LargeValue()
    {
        var result = TimeBar.FormatTime(3661f);
        Assert.Equal("61:01", result);
    }
}
