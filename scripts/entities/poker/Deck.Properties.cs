namespace TimeToTwentyfour.scripts.entities.poker;

public partial class Deck
{
    /// <summary>
    ///     卡册 <see cref="Dictionary{IPokerHolder, IPoker}"/>
    /// </summary>
    private Dictionary<IPokerHolder, IPoker> Album { get; set; } = new();
    
    private IPoker CurrentPoker { get; set; } = null!;
}