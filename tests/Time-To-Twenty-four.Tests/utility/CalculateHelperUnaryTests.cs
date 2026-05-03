using TimeToTwentyfour.scripts.enums.calculator;

namespace TimeToTwentyfour.Tests.utility;

public class CalculateHelperUnaryTests
{
    // ── 绝对值 ──

    [Fact]
    public void AbsoluteValue_Positive_ReturnsSame()
    {
        var poker = Card(5);
        Assert.Equal("5", CalculateHelper.Calculate(poker, ModeType.AbsoluteValue));
    }

    [Fact]
    public void AbsoluteValue_Negative_ReturnsPositive()
    {
        var poker = Card(-5);
        Assert.Equal("5", CalculateHelper.Calculate(poker, ModeType.AbsoluteValue));
    }

    [Fact]
    public void AbsoluteValue_Zero_ReturnsZero()
    {
        var poker = Card(0);
        Assert.Equal("0", CalculateHelper.Calculate(poker, ModeType.AbsoluteValue));
    }

    // ── 阶乘 ──

    [Fact]
    public void Factorial_Five_Returns120()
    {
        var poker = Card(5);
        Assert.Equal("120", CalculateHelper.Calculate(poker, ModeType.Factorial));
    }

    [Fact]
    public void Factorial_Zero_ReturnsOne()
    {
        var poker = Card(0);
        Assert.Equal("1", CalculateHelper.Calculate(poker, ModeType.Factorial));
    }

    [Fact]
    public void Factorial_One_ReturnsOne()
    {
        var poker = Card(1);
        Assert.Equal("1", CalculateHelper.Calculate(poker, ModeType.Factorial));
    }

    [Fact]
    public void Factorial_Negative_ReturnsError()
    {
        var poker = Card(-1);
        Assert.Equal("-1", CalculateHelper.Calculate(poker, ModeType.Factorial));
    }

    [Fact]
    public void Factorial_NonInteger_ReturnsError()
    {
        var poker = Card("3.5", NumType.Decimal);
        Assert.Equal("-1", CalculateHelper.Calculate(poker, ModeType.Factorial));
    }

    // ── 平方根 ──

    [Fact]
    public void SquareRoot_Four_ReturnsTwo()
    {
        var poker = Card(4);
        Assert.Equal("2", CalculateHelper.Calculate(poker, ModeType.SquareRoot));
    }

    [Fact]
    public void SquareRoot_Zero_ReturnsZero()
    {
        var poker = Card(0);
        Assert.Equal("0", CalculateHelper.Calculate(poker, ModeType.SquareRoot));
    }

    [Fact]
    public void SquareRoot_Negative_ReturnsError()
    {
        var poker = Card(-4);
        Assert.Equal("ERROR:InvalidSqrt", CalculateHelper.Calculate(poker, ModeType.SquareRoot));
    }

    // ── 向上取整 ──

    [Fact]
    public void Ceil_Integer_ReturnsSame()
    {
        var poker = Card(3);
        Assert.Equal("3", CalculateHelper.Calculate(poker, ModeType.Ceil));
    }

    [Fact]
    public void Ceil_PositiveDecimal_RoundsUp()
    {
        var poker = Card("3.2", NumType.Decimal);
        Assert.Equal("4", CalculateHelper.Calculate(poker, ModeType.Ceil));
    }

    [Fact]
    public void Ceil_NegativeDecimal_RoundsTowardZero()
    {
        var poker = Card("-1.5", NumType.Decimal);
        Assert.Equal("-1", CalculateHelper.Calculate(poker, ModeType.Ceil));
    }

    // ── 向下取整 ──

    [Fact]
    public void Floor_Integer_ReturnsSame()
    {
        var poker = Card(3);
        Assert.Equal("3", CalculateHelper.Calculate(poker, ModeType.Floor));
    }

    [Fact]
    public void Floor_PositiveDecimal_RoundsDown()
    {
        var poker = Card("3.8", NumType.Decimal);
        Assert.Equal("3", CalculateHelper.Calculate(poker, ModeType.Floor));
    }

    [Fact]
    public void Floor_NegativeDecimal_RoundsAwayFromZero()
    {
        var poker = Card("-1.5", NumType.Decimal);
        Assert.Equal("-2", CalculateHelper.Calculate(poker, ModeType.Floor));
    }
}
