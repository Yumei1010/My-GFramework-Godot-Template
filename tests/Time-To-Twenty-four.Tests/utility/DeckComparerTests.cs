namespace TimeToTwentyfour.Tests.utility;

public class DeckComparerTests
{
    [Fact]
    public void CompareBySuit_DifferentSuits_ReturnsDescendingOrder()
    {
        var heart = Card("1", NumType.Integer);
        heart.SuitType = SuitType.Heart;
        var spade = Card("1", NumType.Integer);
        spade.SuitType = SuitType.Spade;

        // Spade(2) > Heart(0), descending → Spade comes first → a=Heart, b=Spade → positive
        int result = DeckComparer.CompareBySuit(heart, spade);

        Assert.True(result > 0);
    }

    [Fact]
    public void CompareBySuit_SameSuit_ComparesByNumType()
    {
        var a = Card("1", NumType.Integer);
        a.SuitType = SuitType.Heart;
        var b = Card("1", NumType.Decimal);
        b.SuitType = SuitType.Heart;

        // Same suit, Integer(0) < Decimal(2) → negative
        int result = DeckComparer.CompareBySuit(a, b);

        Assert.True(result < 0);
    }

    [Fact]
    public void CompareBySuit_SameSuitAndNumType_ReturnsZero()
    {
        var a = Card("1", NumType.Integer);
        a.SuitType = SuitType.Heart;
        var b = Card("2", NumType.Integer);
        b.SuitType = SuitType.Heart;

        int result = DeckComparer.CompareBySuit(a, b);

        Assert.Equal(0, result);
    }

    [Fact]
    public void CompareBySuit_ClubBeforeDiamond()
    {
        var club = Card("1", NumType.Integer);
        club.SuitType = SuitType.Club;
        var diamond = Card("1", NumType.Integer);
        diamond.SuitType = SuitType.Diamond;

        // Club(3) > Diamond(1), descending → a=Diamond, b=Club → positive
        int result = DeckComparer.CompareBySuit(diamond, club);

        Assert.True(result > 0);
    }

    [Fact]
    public void CompareByRank_DifferentNumTypes_ReturnsAscendingOrder()
    {
        var integer = Card("1", NumType.Integer);
        var fraction = Card("1/2", NumType.Fraction);

        // Integer(0) < Fraction(1) → negative
        int result = DeckComparer.CompareByRank(integer, fraction);

        Assert.True(result < 0);
    }

    [Fact]
    public void CompareByRank_SameNumType_ComparesBySuitDescending()
    {
        var heart = Card("1", NumType.Integer);
        heart.SuitType = SuitType.Heart;
        var spade = Card("2", NumType.Integer);
        spade.SuitType = SuitType.Spade;

        // Same NumType, Spade(2) > Heart(0) descending → a=Heart, b=Spade → positive
        int result = DeckComparer.CompareByRank(heart, spade);

        Assert.True(result > 0);
    }

    [Fact]
    public void CompareByRank_SameNumTypeAndSuit_ReturnsZero()
    {
        var a = Card("1", NumType.Integer);
        a.SuitType = SuitType.Heart;
        var b = Card("2", NumType.Integer);
        b.SuitType = SuitType.Heart;

        int result = DeckComparer.CompareByRank(a, b);

        Assert.Equal(0, result);
    }
}
