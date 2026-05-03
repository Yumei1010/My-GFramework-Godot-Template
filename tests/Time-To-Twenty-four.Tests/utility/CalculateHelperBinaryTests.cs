using TimeToTwentyfour.scripts.enums.calculator;

namespace TimeToTwentyfour.Tests.utility;

public class CalculateHelperBinaryTests
{
    // ── 加法 ──

    [Fact]
    public void Add_TwoIntegers_ReturnsSum()
    {
        var a = Card(1);
        var b = Card(2);
        Assert.Equal("3", CalculateHelper.Calculate(a, b, ModeType.Add));
    }

    [Fact]
    public void Add_TwoFractions_ReturnsFractionSum()
    {
        var a = Card("1/2", NumType.Fraction);
        var b = Card("1/2", NumType.Fraction);
        Assert.Equal("1", CalculateHelper.Calculate(a, b, ModeType.Add));
    }

    [Fact]
    public void Add_NegativeAndPositive_ReturnsCorrectSum()
    {
        var a = Card(-3);
        var b = Card(5);
        Assert.Equal("2", CalculateHelper.Calculate(a, b, ModeType.Add));
    }

    [Fact]
    public void Add_TwoDecimals_ReturnsCorrectSum()
    {
        var a = Card("0.5", NumType.Decimal);
        var b = Card("0.5", NumType.Decimal);
        Assert.Equal("1", CalculateHelper.Calculate(a, b, ModeType.Add));
    }

    // ── 减法 ──

    [Fact]
    public void Subtract_TwoIntegers_ReturnsDifference()
    {
        var a = Card(5);
        var b = Card(3);
        Assert.Equal("2", CalculateHelper.Calculate(a, b, ModeType.Subtract));
    }

    [Fact]
    public void Subtract_ResultNegative_ReturnsNegative()
    {
        var a = Card(1);
        var b = Card(3);
        Assert.Equal("-2", CalculateHelper.Calculate(a, b, ModeType.Subtract));
    }

    [Fact]
    public void Subtract_Fractions_ReturnsFraction()
    {
        var a = Card("3/4", NumType.Fraction);
        var b = Card("1/4", NumType.Fraction);
        Assert.Equal("1/2", CalculateHelper.Calculate(a, b, ModeType.Subtract));
    }

    // ── 乘法 ──

    [Fact]
    public void Multiply_TwoIntegers_ReturnsProduct()
    {
        var a = Card(2);
        var b = Card(3);
        Assert.Equal("6", CalculateHelper.Calculate(a, b, ModeType.Multiply));
    }

    [Fact]
    public void Multiply_ByZero_ReturnsZero()
    {
        var a = Card(5);
        var b = Card(0);
        Assert.Equal("0", CalculateHelper.Calculate(a, b, ModeType.Multiply));
    }

    [Fact]
    public void Multiply_Fractions_ReturnsFraction()
    {
        var a = Card("1/2", NumType.Fraction);
        var b = Card("1/3", NumType.Fraction);
        Assert.Equal("1/6", CalculateHelper.Calculate(a, b, ModeType.Multiply));
    }

    // ── 除法 ──

    [Fact]
    public void Divide_TwoIntegers_ReturnsQuotient()
    {
        var a = Card(6);
        var b = Card(2);
        Assert.Equal("3", CalculateHelper.Calculate(a, b, ModeType.Divide));
    }

    [Fact]
    public void Divide_ResultFraction_ReturnsFraction()
    {
        var a = Card(1);
        var b = Card(2);
        Assert.Equal("1/2", CalculateHelper.Calculate(a, b, ModeType.Divide));
    }

    [Fact]
    public void Divide_ByZero_ReturnsError()
    {
        var a = Card(5);
        var b = Card(0);
        Assert.Equal("ERROR:DivByZero", CalculateHelper.Calculate(a, b, ModeType.Divide));
    }

    // ── 取模 ──

    [Fact]
    public void Modulo_TwoIntegers_ReturnsRemainder()
    {
        var a = Card(5);
        var b = Card(3);
        Assert.Equal("2", CalculateHelper.Calculate(a, b, ModeType.Modulo));
    }

    [Fact]
    public void Modulo_ByZero_ReturnsError()
    {
        var a = Card(5);
        var b = Card(0);
        Assert.Equal("ERROR:DivByZero", CalculateHelper.Calculate(a, b, ModeType.Modulo));
    }

    // ── 指数幂 ──

    [Fact]
    public void Power_PositiveBase_ReturnsCorrectPower()
    {
        var a = Card(2);
        var b = Card(3);
        Assert.Equal("8", CalculateHelper.Calculate(a, b, ModeType.Power));
    }

    [Fact]
    public void Power_ZeroExponent_ReturnsOne()
    {
        var a = Card(5);
        var b = Card(0);
        Assert.Equal("1", CalculateHelper.Calculate(a, b, ModeType.Power));
    }

    [Fact]
    public void Power_ZeroBase_ReturnsZero()
    {
        var a = Card(0);
        var b = Card(3);
        Assert.Equal("0", CalculateHelper.Calculate(a, b, ModeType.Power));
    }

    // ── N 次根 ──

    [Fact]
    public void NthRoot_EightCubeRoot_ReturnsTwo()
    {
        var a = Card(3);
        var b = Card(8);
        Assert.Equal("2", CalculateHelper.Calculate(a, b, ModeType.NthRoot));
    }

    [Fact]
    public void NthRoot_ZeroRootIndex_ReturnsError()
    {
        var a = Card(0);
        var b = Card(8);
        Assert.Equal("ERROR:ZeroRootIndex", CalculateHelper.Calculate(a, b, ModeType.NthRoot));
    }
}
