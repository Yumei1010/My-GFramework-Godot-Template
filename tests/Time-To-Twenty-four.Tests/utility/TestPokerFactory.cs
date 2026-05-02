namespace TimeToTwentyfour.Tests.utility;

internal static class TestPokerFactory
{
    internal static PokerStub Card(int value) => new()
    {
        NumValue = value.ToString(System.Globalization.CultureInfo.InvariantCulture),
        NumType = NumType.Integer
    };

    internal static PokerStub Card(string value, NumType numType) => new()
    {
        NumValue = value,
        NumType = numType
    };
}
