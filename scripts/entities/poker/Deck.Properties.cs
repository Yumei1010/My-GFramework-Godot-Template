namespace TimeToTwentyfour.scripts.entities.poker;

public partial class Deck
{
    /// <summary>
    ///     卡套组 <see cref="IList{IPokerHolder}"/>
    /// </summary>
    private IList<IPokerHolder> Holders { get; set; } = new List<IPokerHolder>();
}