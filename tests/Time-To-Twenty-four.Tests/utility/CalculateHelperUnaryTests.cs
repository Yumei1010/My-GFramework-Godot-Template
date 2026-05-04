using TimeToTwentyfour.scripts.component.calculator.mode;

namespace TimeToTwentyfour.Tests.utility;

public class CalculateHelperUnaryTests
{
    // ── 绝对值 ──

    [Fact]
    public void AbsoluteValue_Positive_ReturnsSame()
    {
        var poker = Card(5);
        Assert.Equal("5", new AbsoluteValueMode().Calculate(poker));
    }

    [Fact]
    public void AbsoluteValue_Negative_ReturnsPositive()
    {
        var poker = Card(-5);
        Assert.Equal("5", new AbsoluteValueMode().Calculate(poker));
    }

    [Fact]
    public void AbsoluteValue_Zero_ReturnsZero()
    {
        var poker = Card(0);
        Assert.Equal("0", new AbsoluteValueMode().Calculate(poker));
    }

    // ── 阶乘 ──

    [Fact]
    public void Factorial_Five_Returns120()
    {
        var poker = Card(5);
        Assert.Equal("120", new FactorialMode().Calculate(poker));
    }

    [Fact]
    public void Factorial_Zero_ReturnsOne()
    {
        var poker = Card(0);
        Assert.Equal("1", new FactorialMode().Calculate(poker));
    }

    [Fact]
    public void Factorial_One_ReturnsOne()
    {
        var poker = Card(1);
        Assert.Equal("1", new FactorialMode().Calculate(poker));
    }

    [Fact]
    public void Factorial_Negative_ReturnsError()
    {
        var poker = Card(-1);
        Assert.Equal("-1", new FactorialMode().Calculate(poker));
    }

    [Fact]
    public void Factorial_NonInteger_ReturnsError()
    {
        var poker = Card("3.5", NumType.Decimal);
        Assert.Equal("-1", new FactorialMode().Calculate(poker));
    }

    // ── 平方根 ──

    [Fact]
    public void SquareRoot_Four_ReturnsTwo()
    {
        var poker = Card(4);
        Assert.Equal("2", new SquareRootMode().Calculate(poker));
    }

    [Fact]
    public void SquareRoot_Zero_ReturnsZero()
    {
        var poker = Card(0);
        Assert.Equal("0", new SquareRootMode().Calculate(poker));
    }

    [Fact]
    public void SquareRoot_Negative_ReturnsError()
    {
        var poker = Card(-4);
        Assert.Equal("ERROR:InvalidSqrt", new SquareRootMode().Calculate(poker));
    }

    // ── 向上取整 ──

    [Fact]
    public void Ceil_Integer_ReturnsSame()
    {
        var poker = Card(3);
        Assert.Equal("3", new CeilMode().Calculate(poker));
    }

    [Fact]
    public void Ceil_PositiveDecimal_RoundsUp()
    {
        var poker = Card("3.2", NumType.Decimal);
        Assert.Equal("4", new CeilMode().Calculate(poker));
    }

    [Fact]
    public void Ceil_NegativeDecimal_RoundsTowardZero()
    {
        var poker = Card("-1.5", NumType.Decimal);
        Assert.Equal("-1", new CeilMode().Calculate(poker));
    }

    // ── 向下取整 ──

    [Fact]
    public void Floor_Integer_ReturnsSame()
    {
        var poker = Card(3);
        Assert.Equal("3", new FloorMode().Calculate(poker));
    }

    [Fact]
    public void Floor_PositiveDecimal_RoundsDown()
    {
        var poker = Card("3.8", NumType.Decimal);
        Assert.Equal("3", new FloorMode().Calculate(poker));
    }

    [Fact]
    public void Floor_NegativeDecimal_RoundsAwayFromZero()
    {
        var poker = Card("-1.5", NumType.Decimal);
        Assert.Equal("-2", new FloorMode().Calculate(poker));
    }
}
