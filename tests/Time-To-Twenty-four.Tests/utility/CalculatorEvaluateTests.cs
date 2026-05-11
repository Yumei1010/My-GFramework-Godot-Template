using TimeToTwentyfour.scripts.component.calculator;
using TimeToTwentyfour.scripts.component.calculator.mode;
using TimeToTwentyfour.scripts.enums.calculator;
using TimeToTwentyfour.scripts.model.poker;

namespace TimeToTwentyfour.Tests.utility;

public class CalculatorEvaluateTests
{
    [Fact]
    public void Evaluate_NullMode_ReturnsNoModeError()
    {
        var hands = new List<IPokerData> { Card(1) };

        var result = Calculator.Evaluate(null!, hands);

        Assert.Equal("ERROR:NoModeSelected", result);
    }

    [Fact]
    public void Evaluate_BinaryMode_InsufficientHands_ReturnsError()
    {
        var hands = new List<IPokerData> { Card(1) };
        var mode = new StubBinaryMode();

        var result = Calculator.Evaluate(mode, hands);

        Assert.Equal("ERROR:InsufficientHands", result);
    }

    [Fact]
    public void Evaluate_BinaryMode_SufficientHands_ReturnsCalculation()
    {
        var hands = new List<IPokerData> { Card(1), Card(2) };
        var mode = new StubBinaryMode();

        var result = Calculator.Evaluate(mode, hands);

        Assert.Equal("1+2", result);
    }

    [Fact]
    public void Evaluate_UnaryMode_InsufficientHands_ReturnsError()
    {
        var hands = new List<IPokerData>();
        var mode = new StubUnaryMode();

        var result = Calculator.Evaluate(mode, hands);

        Assert.Equal("ERROR:InsufficientHands", result);
    }

    [Fact]
    public void Evaluate_UnaryMode_SufficientHands_ReturnsCalculation()
    {
        var hands = new List<IPokerData> { Card(5) };
        var mode = new StubUnaryMode();

        var result = Calculator.Evaluate(mode, hands);

        Assert.Equal("f(5)", result);
    }
}

/// <summary>二元运算测试桩</summary>
internal class StubBinaryMode : IModeStub
{
    public override bool IsBinary => true;
    public override string Calculate(IPokerData a, IPokerData b) => $"{a.NumValue}+{b.NumValue}";
}

/// <summary>一元运算测试桩</summary>
internal class StubUnaryMode : IModeStub
{
    public override bool IsBinary => false;
    public override string Calculate(IPokerData p) => $"f({p.NumValue})";
}

/// <summary>IMode 抽象桩：提供默认实现以减少样板代码</summary>
internal abstract class IModeStub : IMode
{
    public virtual ModeType ModeType => ModeType.Add;
    public abstract bool IsBinary { get; }
    public virtual string Calculate(IPokerData pokerA, IPokerData pokerB)
    {
        throw new NotImplementedException();
    }
    public virtual string Calculate(IPokerData poker)
    {
        throw new NotImplementedException();
    }
}
